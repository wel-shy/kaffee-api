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
    email: string,
    password: string
  ): Promise<IUser> {
    const userRepository: IResourceRepository<
      User
    > = RepositoryFactory.getRepository("user");
    let user: IUser;

    // Get user
    try {
      user = await userRepository.findOneWithFilter({ email });
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
   * @param expiresIn String value for expiration, for 1 day it would be `1 day`
   * @return
   */
  public generateToken(user: IUser, expiresIn?: string): string {
    const payload = {
      email: user.email,
      id: user.id
    };

    if (expiresIn) {
      // create and sign token against the app secret
      return jwt.sign(payload, process.env.SECRET, {
        expiresIn // expires in 24 hours
      });
    }

    return jwt.sign(payload, process.env.REFRESH_SECRET);
  }

  /**
   * Decode a JWT token.
   *
   * @param token jwt token
   * @param refresh refresh token flag
   */
  public decodeToken(token: string, refresh?: boolean): Promise<any> {
    const secret = refresh ? process.env.REFRESH_SECRET : process.env.SECRET;

    return new Promise((resolve, reject) => {
      jwt.verify(token, secret, (err: Error, data: any) => {
        if (err) {
          reject(new Unauthorized("Token invalid"));
        } else {
          return resolve(data);
        }
      });
    });
  }
}
