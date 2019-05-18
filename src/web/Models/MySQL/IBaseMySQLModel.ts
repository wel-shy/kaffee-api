import { BaseEntity } from "typeorm";
import IBaseResource from "../IBaseModel";

/**
 * Base mysql resource
 */
export default interface IBaseMySQLModel extends IBaseResource, BaseEntity {}
