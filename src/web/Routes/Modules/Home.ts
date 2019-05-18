import { NextFunction, Request, Response } from "express";
import { HttpMethods } from "../../HttpMethods";
import { BaseRouter } from "../BaseRouter";

/**
 * HomeRouter
 *
 * Serves endpoints after '/'
 */
export class Home extends BaseRouter {
  constructor() {
    super();
    this.addRoute("/", HttpMethods.GET, this.showHome);
  }

  /**
   * Return a message
   *
   * @param {e.Request} req
   * @param {e.Response} res
   * @param {e.NextFunction} next
   * @returns {e.Response}
   */
  public showHome(req: Request, res: Response, next: NextFunction): Response {
    return res.send("Hello World");
  }
}
