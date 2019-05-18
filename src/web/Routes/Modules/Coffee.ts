import { NextFunction, Request, Response } from "express";
import { HttpMethods } from "../../HttpMethods";
import { BaseRouter } from "../BaseRouter";
import RepositoryFactory from "../../Repositories/RepositoryFactory";
import { IUser } from "../../Models/IUser";
import { InternalServerError, NotFound } from "@curveball/http-errors/dist";
import { Coffee } from "../../Models/MySQL/Coffee";
import { Reply } from "../../Reply";
import { checkToken } from "../../Middleware/Authenticate";
import { User } from "../../Models/MySQL/User";

export default class CoffeeRouter extends BaseRouter {
  constructor() {
    super();
    this.addMiddleware(checkToken);
    this.addRoute("/", HttpMethods.POST, this.store);
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
      console.log(JSON.stringify(coffee, null, 2));
      coffee = await coffeeRepository.store(coffee);
    } catch (e) {
      console.error(e);
      return next(new InternalServerError("Could not store coffee"));
    }

    return res.json(new Reply(200, "success", false, { coffee }));
  }
}
