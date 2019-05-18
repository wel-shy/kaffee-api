import * as crypto from "crypto";

/**
 * Useful crypto functions
 * todo: move to a controller
 */
export default class CryptoHelper {
  /**
   * Hash a string with sha256
   *
   * @param {string} m
   * @param {string} salt
   * @returns {string}
   */
  public static hashString(m: string, salt?: string): string {
    const hash: crypto.Hash = crypto.createHash("sha256");

    if (salt) {
      hash.update(`${salt}${m}`);
    } else {
      hash.update(m);
    }

    return hash.digest("hex");
  }

  /**
   * Get a random string of n length
   *
   * @param {number} length
   * @returns {string}
   */
  public static getRandomString(length: number): string {
    return crypto.randomBytes(length).toString("hex");
  }
}
