import { IUser } from "./IUser";

/**
 * Interface for a user.
 */
export interface ICoffee {
  id: number;
  user: IUser;
  createdAt: Date;
  from: string;

  getId(): number;
  toJSONObject(): {};
}
