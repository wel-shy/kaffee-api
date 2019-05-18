import {
  BaseEntity,
  Column,
  Entity,
  ManyToOne,
  PrimaryGeneratedColumn,
  JoinColumn
} from "typeorm";
import IBaseModel from "../IBaseModel";
import { ICoffee } from "../ICoffee";
import { User } from "./User";

/**
 * Coffee MySQL implementation
 */
@Entity()
export class Coffee extends BaseEntity implements IBaseModel, ICoffee {
  @PrimaryGeneratedColumn()
  public id: number;

  @ManyToOne((type: any) => User, (user: any) => user.coffees)
  public user: User;

  @Column({
    default: () => "NOW()",
    type: "timestamp"
  })
  public createdAt: Date;

  @Column("varchar")
  public from: string;

  public getId(): number {
    return this.id;
  }

  /**
   * Convert to a json object for record updating and insertion
   * @returns {{}}
   */
  public toJSONObject(): {} {
    return {
      createdAt: this.createdAt,
      from: this.from,
      id: this.id,
      user: this.user.toJSONObject()
    };
  }
}
