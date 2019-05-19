import {
  BaseEntity,
  Column,
  Entity,
  OneToMany,
  PrimaryGeneratedColumn
} from "typeorm";
import IBaseModel from "../IBaseModel";
import { IUser } from "../IUser";
import { Coffee } from "./Coffee";

/**
 * User MySQL implementation
 */
@Entity()
export class User extends BaseEntity implements IBaseModel, IUser {
  @PrimaryGeneratedColumn()
  public id: number;

  @Column({
    type: "varchar",
    unique: true
  })
  public email: string;

  @Column("varchar")
  public password: string;

  @Column("varchar")
  public iv: string;

  @Column({
    nullable: true,
    type: "varchar"
  })
  public refreshToken: string;

  @OneToMany((type: any) => Coffee, (coffee: any) => coffee.user)
  public coffees: Coffee[];

  public getId(): number {
    return this.id;
  }

  /**
   * Convert to a json object for record updating and insertion
   * @returns {{}}
   */
  public toJSONObject(): {} {
    return {
      email: this.email,
      id: this.id,
      iv: this.iv
    };
  }
}
