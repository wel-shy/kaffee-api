import { InternalServerError, NotFound } from "@curveball/http-errors/dist";
import { NextFunction, Request, Response } from "express";
import { HttpMethods } from "../../HttpMethods";
import { checkToken } from "../../Middleware/Authenticate";
import { ICoffee } from "../../Models/ICoffee";
import { IUser } from "../../Models/IUser";
import { Coffee } from "../../Models/MySQL/Coffee";
import { User } from "../../Models/MySQL/User";
import { Reply } from "../../Reply";
import { IResourceRepository } from "../../Repositories/IResourceRepository";
import MYSQLCoffeeRepository from "../../Repositories/MYSQLCoffeeRepository";
import RepositoryFactory from "../../Repositories/RepositoryFactory";
import { BaseRouter } from "../BaseRouter";

export default class CoffeeRouter extends BaseRouter {
  constructor() {
    super();
    this.addMiddleware(checkToken);
    this.addRoute("/", HttpMethods.POST, this.store);
    this.addRoute("/count", HttpMethods.GET, this.count);
    this.addRoute("/", HttpMethods.GET, this.index);
    this.addRoute("/date", HttpMethods.GET, this.dailyCount);
  }

  /**
   * Store a coffee
   *
   * @param req Request
   * @param res Response
   * @param next NextFunction
   */
  public async store(
    req: Request,
    res: Response,
    next: NextFunction
  ): Promise<Response | void> {
    const coffeeRepository = RepositoryFactory.getRepository("coffee");
    const userRepository = RepositoryFactory.getRepository("user");
    let user: IUser;
    let from: string = req.params.from;

    if (!from || from === "") {
      from = "webapp";
    }

    try {
      user = await userRepository.get(res.locals.user.id);
    } catch (e) {
      return next(new InternalServerError("Couldnt get user"));
    }

    if (!user) {
      return next(new NotFound("User not found"));
    }

    let coffee: Coffee = new Coffee();
    coffee.from = from;
    coffee.user = user as User;

    try {
      coffee = await coffeeRepository.store(coffee);
    } catch (e) {
      return next(new InternalServerError("Could not store coffee"));
    }

    return res.json(new Reply(200, "success", false, { coffee }));
  }

  public async count(
    req: Request,
    res: Response,
    next: NextFunction
  ): Promise<Response | void> {
    const userRepository = RepositoryFactory.getRepository("user");
    const coffeeController: IResourceRepository<
      Coffee
    > = RepositoryFactory.getRepository("coffee");
    let user: IUser;
    let coffees: ICoffee[];

    try {
      user = await userRepository.get(res.locals.user.id);
      coffees = await coffeeController.findManyWithFilter({
        user: user.getId()
      });
    } catch (e) {
      return next(
        new InternalServerError("Something went wrong fetching resources")
      );
    }

    return res.json(
      new Reply(200, "success", false, { count: coffees.length })
    );
  }

  public async index(
    req: Request,
    res: Response,
    next: NextFunction
  ): Promise<Response | void> {
    const userRepository = RepositoryFactory.getRepository("user");
    const coffeeRepository: IResourceRepository<
      Coffee
    > = RepositoryFactory.getRepository("coffee");
    let user: IUser;
    let coffees: ICoffee[];

    try {
      user = await userRepository.get(res.locals.user.id);
      coffees = await coffeeRepository.findManyWithFilter({
        user: user.getId()
      });
    } catch (e) {
      return next(
        new InternalServerError("Something went wrong fetching resources")
      );
    }

    return res.json(new Reply(200, "success", false, { count: coffees }));
  }

  public async dailyCount(
    req: Request,
    res: Response,
    next: NextFunction
  ): Promise<Response | void> {
    const coffeeRepository: MYSQLCoffeeRepository = new MYSQLCoffeeRepository(
      Coffee
    );

    const coffeeCount = await coffeeRepository.coffeePerDate(
      res.locals.user.id
    );
    return res.json(new Reply(200, "success", false, coffeeCount));
  }
}
