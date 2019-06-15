import { json } from "body-parser";
import { FindConditions, getManager, Like, ObjectType } from "typeorm";
import { Coffee } from "../Models/MySQL/Coffee";
import { MySQLResourceRepository } from "./MySQLResourceRepository";

export default class MYSQLCoffeeRepository extends MySQLResourceRepository<
  Coffee
> {
  public async coffeePerDate(userId: number): Promise<ICoffeeCount> {
    const coffees = await getManager().getRepository(Coffee).query(`
        SELECT COUNT(*) as count, DATE(c.createdAt) as d
        FROM coffee c
        WHERE userId = ${userId}
        GROUP BY DATE(c.createdAt)
      `);

    console.log(JSON.stringify(coffees, null, 2));

    return (coffees as unknown) as ICoffeeCount;
  }
}

export interface ICoffeeCount {
  count: number;
  d: Date;
}
