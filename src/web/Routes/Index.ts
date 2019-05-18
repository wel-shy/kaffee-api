import { Express } from "express";
import { Auth } from "./Modules/Auth";
import { Home } from "./Modules/Home";

/**
 * Add routes to the router.
 *
 * @param {e.Express} app
 * @returns {e.Express}
 */
export default function addRoutes(app: Express): Express {
  app.use("/", new Home().getRouter());
  app.use("/api/auth", new Auth().getRouter());

  return app;
}
