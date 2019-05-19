import { Connection, createConnection } from "typeorm";
import { Coffee } from "../Models/MySQL/Coffee";
import { User } from "../Models/MySQL/User";

/**
 * Get a connection to the MySQL database.
 */
export async function getMySQLConnection(): Promise<Connection> {
  return await createConnection({
    database: process.env.DB_DATABASE,
    entities: [User, Coffee],
    host: process.env.DB_HOST,
    logging: false,
    password: process.env.DB_PASSWORD,
    port: parseInt(process.env.DB_PORT, 10),
    synchronize: true,
    type: "mysql",
    username: process.env.DB_USERNAME
  });
}
