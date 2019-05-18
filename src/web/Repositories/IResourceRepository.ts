import IBaseModel from "../Models/IBaseModel";

/**
 * Interface for resource controllers.
 * All controllers for all dbs should implement this if they manage a resource.
 */
export interface IResourceRepository<T extends IBaseModel> {
  /**
   * Get a single resource
   *
   * @param {number} id
   * @returns {Promise<T>}
   */
  get(id: number): Promise<T>;

  /**
   * Get all resources
   *
   * @returns {Promise<T[]>}
   */
  getAll(): Promise<T[]>;

  /**
   * Edit a resource
   *
   * @param {number} id
   * @param data
   * @returns {Promise<T>}
   */
  edit(id: number, data: any): Promise<T>;

  /**
   * Store a resource
   *
   * @param data
   * @returns {Promise<T>}
   */
  store(data: any): Promise<T>;

  /**
   * Destroy a resource
   *
   * @param {number} id
   * @returns {Promise<void>}
   */
  destroy(id: number): Promise<void>;

  /**
   * Find a single resource with a filter
   *
   * @param {{}} filter
   * @returns {Promise<T>}
   */
  findOneWithFilter(filter: {}): Promise<T>;

  /**
   * Find a group of resources matching filter.
   * @param {{}} filter
   * @param {{limit: number; skip: number}} options
   * @returns {Promise<T[]>}
   */
  findManyWithFilter(
    filter: {},
    options?: {
      limit: number;
      skip: number;
    }
  ): Promise<T[]>;

  /**
   * Search for a resource where field contains query
   *
   * @param {string} field
   * @param {string} query
   * @returns {Promise<T[]>}
   */
  search(field: string, query: string, filter: {}): Promise<T[]>;

  /**
   * Get a count of all resources matching filter.
   *
   * @param {{}} filter
   * @returns {Promise<number>}
   */
  getCount(filter: {}): Promise<number>;

  /**
   * Set the table the router resource belongs to
   *
   * @param {string} table
   */
  setTableName(table: string): void;

  /**
   * Get the table name the router resource belongs to
   *
   * @returns {string}
   */
  getTableName(): string;
}
