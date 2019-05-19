import { ICoffee } from "./ICoffee";

/**
 * Interface for a user.
 */
export interface IUser {
  id: number;
  email: string;
  password: string;
  iv: string;
  coffees: ICoffee[];
  refreshToken: string;

  getId(): number;
  toJSONObject(): {};
}
