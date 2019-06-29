using Kaffee.Models;
using Kaffee.Settings;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace Kaffee.Services
{
    /// <summary>
    ///  Model a Mongo Collection for the <see cref="User"/> model. 
    /// </summary>
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Coffee> _coffees;

        /// <summary>
        /// Get a new instance of the user service.
        /// </summary>
        /// <param name="settings">Database settings.</param>
        public UserService(IKaffeeDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName, new MongoDatabaseSettings {
            
            });

            _users = database.GetCollection<User>(settings.UserCollectionName);
            _coffees = database.GetCollection<Coffee>(settings.CoffeeCollectionName);
        }

        /// <summary>
        /// Get a user by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Matching user.</returns>
        public User Get(string id) =>
            _users.Find<User>(user => user.Id == id).FirstOrDefault();

        /// <summary>
        /// Get a user by their refresh token
        /// </summary>
        /// <param name="token">refresh token</param>
        /// <returns>Matching user.</returns>
        public Task<User> GetWithToken(string token) =>
            _users.Find<User>(user => user.RefreshToken == token).FirstOrDefaultAsync();

        /// <summary>
        /// Get a user by their email.
        /// </summary>
        /// <param name="email">User's email address.</param>
        /// <returns>Matching user.</returns>
        public Task<User> GetWithEmail(string email) =>
            _users.Find<User>(user => user.Email == email).FirstOrDefaultAsync();

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <returns>The new user.</returns>
        public async Task<User> Create(User user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }
        
        /// <summary>
        /// Update a new user.
        /// </summary>
        /// <param name="id">User's id</param>
        /// <param name="userIn">Details to update user to.</param>
        public void Update(string id, User userIn) =>
            _users.ReplaceOne(user => user.Id == id, userIn);

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="id"></param>
        public async Task Remove(string id) 
        {
            await _coffees.DeleteManyAsync((coffee) => coffee.UserId.Equals(id));
            _users.DeleteOne(user => user.Id == id);
        }
    }
}