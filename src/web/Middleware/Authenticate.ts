import { Unauthorized } from "@curveball/http-errors";
import * as express from "express";
import * as jwt from "jsonwebtoken";
import { IUser } from "../Models/IUser";

/**
 * Check the users access token.
 * Should reject if there is no token, or an invalid token.
 *
 * @param {e.Request} req
 * @param {e.Response} res
 * @param {e.NextFunction} next
 */
export function checkToken(
  req: express.Request,
  res: express.Response,
  next: express.NextFunction
) {
  const token: string =
    req.body.token ||
    req.query.token ||
    req.headers["x-access-token"] ||
    req.params.token;

  // If there is token then verify
  if (token) {
    jwt.verify(token, process.env.SECRET, (err: Error, user: IUser) => {
      if (err) {
        // Add error to locals
        res.locals.error = new Unauthorized(err.message);
        return next();
      } else {
        // Add user to locals and move to next function if token is valid
        res.locals.user = user;
        return next();
      }
    });
  } else {
    // No token, add error and move to error handler
    res.locals.error = new Unauthorized("no token provided");
    return next();
  }
}
