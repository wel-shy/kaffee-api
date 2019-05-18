import { InternalServerError, Unauthorized } from "@curveball/http-errors";
import * as jwt from "jsonwebtoken";
import CryptoHelper from "../CryptoHelper";
import { IUser } from "../Models/IUser";
import { User } from "../Models/MySQL/User";
import { IResourceRepository } from "../Repositories/IResourceRepository";
import RepositoryFactory from "../Repositories/RepositoryFactory";

/**
 * AuthController
 * Handles authenticating user.
 */
export default class AuthController {
  /**
   * Authenticate a user
   * @param  username username
   * @param  password password
   * @return {IMongoUser} Matched user
   */
  public async authenticateUser(
    username: string,
    password: string
  ): Promise<IUser> {
    const userRepository: IResourceRepository<
      User
    > = RepositoryFactory.getRepository("user");
    let user: IUser;

    // Get user
    try {
      user = await userRepository.findOneWithFilter({ username });
    } catch (error) {
      // Throw if db failure
      throw new InternalServerError(error.message);
    }

    // Throw 401 if username is incorrect
    if (!user) {
      throw new Unauthorized("Username does not exist");
    }

    const hashedPassword: string = CryptoHelper.hashString(password, user.iv);

    // Compare passwords and abort if no match
    if (user.password !== hashedPassword) {
      throw new Unauthorized("Username or password is incorrect");
    }

    return user;
  }

  /**
   * Create a JWT token for the user
   * @param  user IMongoUser
   * @return
   */
  public generateToken(user: IUser): string {
    const payload = {
      id: user.id,
      username: user.email
    };
    // create and sign token against the app secret
    return jwt.sign(payload, process.env.SECRET, {
      expiresIn: "1 day" // expires in 24 hours
    });
  }
}
