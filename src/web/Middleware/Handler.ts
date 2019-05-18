import { HttpError } from "@curveball/http-errors";
import * as express from "express";
import { Reply } from "../Reply";

/**
 * Handle an error and give an appropriate response
 * @param {Error} err
 * @param {e.Request} req
 * @param {e.Response} res
 * @param {e.NextFunction} next
 * @returns {Response}
 */
export const handleResponse: express.ErrorRequestHandler = (
  err: Error,
  req: express.Request,
  res: express.Response,
  next: express.NextFunction
) => {
  const e: HttpError = err as HttpError;
  // Get error message from code
  const reply: Reply = new Reply(e.httpStatus, err.message, true, null);

  // Give full stacktrace if in debug mode
  if (process.env.DEBUG === "true") {
    reply.payload = e.stack;
  }

  // Do not print full stack if unit testing
  if (process.env.TEST !== "true") {
    console.error(e);
  }

  // Set status code of error message
  res.status(e.httpStatus);
  return res.json(reply);
};
