import { BaseEntity, Column, Entity, PrimaryGeneratedColumn } from "typeorm";
import IBaseModel from "../IBaseModel";
import { IUser } from "../IUser";

/**
 * User MySQL implementation
 */
@Entity()
export class User extends BaseEntity implements IBaseModel, IUser {
  @PrimaryGeneratedColumn()
  public id: number;

  @Column("varchar")
  public email: string;

  @Column("varchar")
  public password: string;

  @Column("varchar")
  public iv: string;

  @Column("int")
  public coffeeCount: number;

  public getId(): number {
    return this.id;
  }

  /**
   * Convert to a json object for record updating and insertion
   * @returns {{}}
   */
  public toJSONObject(): {} {
    return {
      coffeeCount: this.coffeeCount,
      email: this.email,
      id: this.id,
      iv: this.iv,
      password: this.password
    };
  }
}
