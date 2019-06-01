import { InternalServerError } from "@curveball/http-errors/dist";
import { NextFunction, Request, Response } from "express";
import { HttpMethods } from "../../HttpMethods";
import { checkToken } from "../../Middleware/Authenticate";
import { ICoffee } from "../../Models/ICoffee";
import { IUser } from "../../Models/IUser";
import { Reply } from "../../Reply";
import RepositoryFactory from "../../Repositories/RepositoryFactory";
import { BaseRouter } from "../BaseRouter";

export default class User extends BaseRouter {
  constructor() {
    super();
    this.addMiddleware(checkToken);
    this.addRoute("/", HttpMethods.DELETE, this.destroy);
  }

  /**
   * Destroy a user.
   *
   * @param req Request
   * @param res Response
   * @param next NextFunction
   */
  public async destroy(
    req: Request,
    res: Response,
    next: NextFunction
  ): Promise<Response | void> {
    const userRepository = RepositoryFactory.getRepository("user");
    const coffeeRepository = RepositoryFactory.getRepository("coffee");
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

    try {
      coffees.forEach(async coffee => {
        await coffeeRepository.destroy(coffee.id);
      });
    } catch (e) {
      return next(new InternalServerError("Couldnt delete coffees"));
    }

    try {
      await userRepository.destroy(user.id);
    } catch (e) {
      return next(new InternalServerError("Couldnt delete user"));
    }

    return res.json(new Reply(200, "success", false, null));
  }
}
