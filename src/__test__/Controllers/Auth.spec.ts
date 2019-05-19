import * as dotenv from "dotenv";
import * as jwt from "jsonwebtoken";
import AuthController from "../../web/Controllers/AuthController";
import { IUser } from "../../web/Models/IUser";
import { User } from "../../web/Models/MySQL/User";

dotenv.config();

describe("AuthController.ts", () => {
  let authController!: AuthController;

  beforeEach(() => {
    authController = new AuthController();
  });

  describe(".generateToken", () => {
    it("Should generate a valid jwt token that expires in 1 day", () => {
      const temp: IUser = new User();
      temp.email = "mail@mail.com";
      temp.id = 1;

      const token: string = authController.generateToken(temp, "1 day");

      jwt.verify(token, process.env.SECRET, (err: Error, data: any) => {
        if (err) {
          throw err;
        } else {
          expect(data.email).toEqual(temp.email);
        }
      });
    });
  });

  describe(".decodeToken", () => {
    it("Should decode a token and return encoded data", async () => {
      const token: string = jwt.sign(
        {
          email: "mail@mail.com",
          id: 1
        },
        process.env.SECRET
      );

      const decoded: any = await authController.decodeToken(token);
      expect(decoded.email).toEqual("mail@mail.com");
    });
  });
});
