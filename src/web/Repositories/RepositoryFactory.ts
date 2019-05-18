import { BaseEntity } from "typeorm";
import IBaseMySQLModel from "../Models/MySQL/IBaseMySQLModel";
import { User } from "../Models/MySQL/User";
import { IResourceRepository } from "./IResourceRepository";
import { MySQLResourceRepository } from "./MySQLResourceRepository";

/**
 * Generate a controller for the type of database
 */
export default class RepositoryFactory {
  /**
   * Determine database type and return fitting controller.
   * @param {string} resName
   * @returns {IResourceRepository<any>}
   */
  public static getRepository(resName: string): IResourceRepository<any> {
    return RepositoryFactory.getMySQLRepository(resName);
  }

  /**
   * Get a MySQL repo for the resource.
   * @param {string} res
   * @returns {MySQLResourceRepository<IBaseMySQLResource>}
   */
  private static getMySQLRepository(
    res: string
  ): MySQLResourceRepository<IBaseMySQLModel> {
    let repository: MySQLResourceRepository<IBaseMySQLModel>;
    switch (res) {
      case "user":
        repository = new MySQLResourceRepository<User>(User);
        break;
      default:
        repository = new MySQLResourceRepository<IBaseMySQLModel>(BaseEntity);
        break;
    }

    repository.setTableName(res);
    return repository;
  }
}
