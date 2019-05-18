import { FindConditions, getManager, Like, ObjectType } from "typeorm";
import IBaseMySQLModel from "../Models/MySQL/IBaseMySQLModel";
import { IResourceRepository } from "./IResourceRepository";

/**
 * Generic controller for resource of type T, must extend typeorm.BaseEntity.
 */
export class MySQLResourceRepository<T extends IBaseMySQLModel>
  implements IResourceRepository<T> {
  private type: ObjectType<T>;
  private table: string;

  constructor(type: ObjectType<T>) {
    this.type = type;
  }

  /**
   * Get the resource by id.
   *
   * @param {number} id
   * @returns {Promise<T: BaseEntity>}
   */
  public async get(id: number): Promise<T> {
    return (await getManager()
      .getRepository(this.type)
      .findOne(id)) as T;
  }

  /**
   * Get all instances of resource.
   *
   * Get all entities
   * @returns {Promise<T[]: BaseEntity[]>}
   */
  public async getAll(): Promise<T[]> {
    return (await getManager()
      .getRepository(this.type)
      .find()) as T[];
  }

  /**
   * Destroy the resource.
   *
   * @param {number} id
   * @returns {Promise<void>}
   */
  public async destroy(id: number): Promise<void> {
    const entity: T = await this.get(id);
    if (entity) {
      await entity.remove();
    }
  }

  /**
   * Update a record
   *
   * @param {number} id
   * @param data
   * @returns {Promise<T>}
   */
  public async edit(id: number, data: any): Promise<T> {
    const entity: T = await this.get(id);
    const entObj: any = entity.toJSONObject(); // Create an obj of the entity

    // Overwrite fields with any new values
    Object.keys(data).forEach((key: string) => {
      entObj[key] = data[key];
    });

    // Store
    return await getManager()
      .getRepository(this.type)
      .save(entObj);
  }

  /**
   * Find many resources matching filter.
   *
   * @param {{}} filter
   * @param {{limit: number; skip: number}} options
   * @returns {Promise<T[]>}
   */
  public async findManyWithFilter(
    filter: {},
    options?: { limit: number; skip: number }
  ): Promise<T[]> {
    const queries: string[] = [];

    // Join filter fields into a where query string
    Object.keys(filter).forEach((key: string) => {
      queries.push(`${this.getTableName()}.${key} = :${key}`);
    });
    const whereQuery = queries.join(" AND ");

    // Apply skip and limit if present
    if (options) {
      return (await getManager()
        .getRepository(this.type)
        .createQueryBuilder(this.getTableName())
        .where(whereQuery, filter)
        .take(options.limit)
        .skip(options.skip)
        .getMany()) as T[];
    }

    return (await getManager()
      .getRepository(this.type)
      .createQueryBuilder(this.getTableName())
      .where(whereQuery, filter)
      .getMany()) as T[];
  }

  /**
   * Find one resource matching a filter
   *
   * @param {{}} filter
   * @returns {Promise<T>}
   */
  public async findOneWithFilter(filter: {}): Promise<T> {
    const queries: string[] = [];

    // Create where query string
    Object.keys(filter).forEach((key: string) => {
      queries.push(`${this.getTableName()}.${key} = :${key}`);
    });
    const whereQuery = queries.join(" AND ");

    // Fetch result
    return (await getManager()
      .getRepository(this.type)
      .createQueryBuilder(this.getTableName())
      .where(whereQuery, filter)
      .getOne()) as T;
  }

  /**
   * Get a count of all resources
   *
   * @param {{}} filter
   * @returns {Promise<number>}
   */
  public async getCount(filter: {}): Promise<number> {
    return (await this.findManyWithFilter(filter)).length;
  }

  /**
   * Get the table name
   *
   * @returns {string}
   */
  public getTableName(): string {
    return this.table;
  }

  /**
   * Set the table name
   *
   * @param {string} table
   */
  public setTableName(table: string): void {
    this.table = table;
  }

  /**
   * Store a resource
   *
   * @param data
   * @returns {Promise<T>}
   */
  public async store(data: any): Promise<T> {
    let entity: T;

    try {
      entity = await getManager()
        .getRepository(this.type)
        .save(data);
    } catch (e) {
      throw e;
    }
    return entity;
  }

  /**
   * Search for a resource where field contains query
   *
   * @param {string} field
   * @param {string} query
   * @param {{}} filter
   * @returns {Promise<T[]>}
   */
  public async search(field: string, query: string, filter: {}): Promise<T[]> {
    const q: any = filter;
    q[field] = Like(`%${query}%`);

    return (await getManager()
      .getRepository(this.type)
      .find(q as FindConditions<{}>)) as T[];
  }
}
