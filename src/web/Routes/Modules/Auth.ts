import {
  BadRequest,
  Forbidden,
  InternalServerError,
  NotFound
} from "@curveball/http-errors";
import { NextFunction, Request, Response } from "express";
import AuthController from "../../Controllers/AuthController";
import CryptoHelper from "../../CryptoHelper";
import { HttpMethods, HttpMethods as Methods } from "../../HttpMethods";
import { IUser } from "../../Models/IUser";
import { Reply } from "../../Reply";
import { IResourceRepository } from "../../Repositories/IResourceRepository";
import RepositoryFactory from "../../Repositories/RepositoryFactory";
import { BaseRouter } from "../BaseRouter";

/**
 * AuthRouter
 *
 * Handles authentication routes.
 */
export class Auth extends BaseRouter {
  /**
   * Add routes to the Auth Router.
   */
  constructor() {
    super();
    this.addRoute("/authenticate", Methods.POST, this.authenticateUser);
    this.addRoute("/register", Methods.POST, this.registerUser);
    this.addRoute("/token", HttpMethods.GET, this.getToken);
  }

  /**
   * Authenticate the user against email and password
   *
   * @param {e.Request} req
   * @param {e.Response} res
   * @param {e.NextFunction} next
   * @returns {Promise<e.Response | void>}
   */
  public async authenticateUser(
    req: Request,
    res: Response,
    next: NextFunction
  ): Promise<Response | void> {
    const authController: AuthController = new AuthController();
    // Get username and password from request
    const email: string = req.body.email;
    const password: string = req.body.password;
    let user: IUser;

    // Try to authenticate
    try {
      user = await authController.authenticateUser(email, password);
    } catch (error) {
      // Throw auth errors
      return next(error);
    }

    // Generate a token
    const token = authController.generateToken(user, "1 day");

    // Return token
    const response = new Reply(200, "success", false, {
      refreshToken: user.refreshToken,
      token
    });
    return res.json(response);
  }

  /**
   * Generate a token from a refresh token.
   *
   * @param req Request
   * @param res Response
   * @param next NextFunction
   */
  public async getToken(
    req: Request,
    res: Response,
    next: NextFunction
  ): Promise<Response | void> {
    const authController: AuthController = new AuthController();
    const userController: IResourceRepository<
      IUser
    > = RepositoryFactory.getRepository("user");
    const refreshToken: string = req.body.refreshToken;
    let user: IUser;
    let decoded: any;

    // Check for refresh token
    if (!refreshToken) {
      return next(new BadRequest("refreshToken missing"));
    }

    // Decode the refresh token
    try {
      decoded = await authController.decodeToken(refreshToken, true);
    } catch (e) {
      return next(e);
    }

    // Fetch the user
    try {
      user = await userController.get(decoded.id);
    } catch (e) {
      return next(new InternalServerError("Could not fetch user"));
    }

    // Abort if user not found
    if (!user) {
      return next(new NotFound("user not found"));
    }

    // Generate a token
    const token = authController.generateToken(user, "1 day");

    // Return a token
    return res.json(new Reply(200, "success", true, { token }));
  }

  /**
   * Register the user
   *
   * @param {e.Request} req
   * @param {e.Response} res
   * @param {e.NextFunction} next
   * @returns {Promise<e.Response | void>}
   */
  public async registerUser(
    req: Request,
    res: Response,
    next: NextFunction
  ): Promise<Response | void> {
    const authController: AuthController = new AuthController();
    // Get username and password
    const email: string = req.body.email;
    const password: string = req.body.password;
    const iv: string = CryptoHelper.getRandomString(16);
    const userRepository: IResourceRepository<
      IUser
    > = RepositoryFactory.getRepository("user");
    let user: IUser;
    let refreshToken: string;

    // abort if either username or password are null
    if (!email || !password) {
      return next(new BadRequest("email or password cannot be empty"));
    }

    // Generate the hashed password
    const hash: string = CryptoHelper.hashString(password, iv);

    // Store the user
    try {
      user = await userRepository.store({ email, password: hash, iv });
    } catch (error) {
      // If user name already exists throw 403
      if (error.message.indexOf("duplicate key error") > -1) {
        return next(new Forbidden(error.message));
      }
      return next(error);
    }

    try {
      refreshToken = authController.generateToken(user);
      await userRepository.edit(user.id, {
        refreshToken
      });
    } catch (e) {
      return next(
        new InternalServerError("Could not store user's refresh token")
      );
    }

    // Generate token
    const token = authController.generateToken(user, "1 day");

    // Return new user and token
    const response = new Reply(200, "success", false, {
      refreshToken,
      token,
      user
    });
    return res.json(response);
  }
}
