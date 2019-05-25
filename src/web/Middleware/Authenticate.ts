import { Unauthorized } from "@curveball/http-errors";
import * as express from "express";
import * as jwt from "jsonwebtoken";
import { IUser } from "../Models/IUser";
import AuthController from "../Controllers/AuthController";

/**
 * Check the users access token.
 * Should reject if there is no token, or an invalid token.
 *
 * @param {e.Request} req
 * @param {e.Response} res
 * @param {e.NextFunction} next
 */
export async function checkToken(
  req: express.Request,
  res: express.Response,
  next: express.NextFunction
) {
  const token: string =
    req.body.token ||
    req.query.token ||
    req.headers["x-access-token"] ||
    req.params.token;
  let decoded: any;

  // If there is token then verify
  if (token) {
    try {
      decoded = await new AuthController().decodeToken(token);
    } catch (e) {
      return next(e);
    }

    res.locals.user = decoded;
    return next();
  } else {
    // No token, add error and move to error handler
    res.locals.error = new Unauthorized("no token provided");
    return next();
  }
}
