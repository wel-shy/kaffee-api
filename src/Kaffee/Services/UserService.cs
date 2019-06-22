using Kaffee.Models;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace Kaffee.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IKaffeeDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName, new MongoDatabaseSettings {
            
            });

            _users = database.GetCollection<User>(settings.UserCollectionName);
        }

        public User Get(string id) =>
            _users.Find<User>(user => user.Id == id).FirstOrDefault();

        public Task<User> GetWithToken(string token) =>
            _users.Find<User>(user => user.RefreshToken == token).FirstOrDefaultAsync();

        public Task<User> GetWithEmail(string email) =>
            _users.Find<User>(user => user.Email == email).FirstOrDefaultAsync();

        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }
        public void Update(string id, User userIn) =>
            _users.ReplaceOne(user => user.Id == id, userIn);

        public void Remove(User userIn) =>
            _users.DeleteOne(user => user.Id == userIn.Id);

        public void Remove(string id) => 
            _users.DeleteOne(user => user.Id == id);
    }
}