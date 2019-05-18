import { Express } from "express";
import { Auth } from "./Modules/Auth";
import { Home } from "./Modules/Home";
import User from "./Modules/User";
import { checkToken } from "../Middleware/Authenticate";
import CoffeeRouter from "./Modules/Coffee";

/**
 * Add routes to the router.
 *
 * @param {e.Express} app
 * @returns {e.Express}
 */
export default function addRoutes(app: Express): Express {
  app.use("/", new Home().getRouter());
  app.use("/api/auth", new Auth().getRouter());
  app.use("/api/user", new User().getRouter());
  app.use("/api/coffee", new CoffeeRouter().getRouter());

  return app;
}
