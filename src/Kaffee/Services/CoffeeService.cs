using Kaffee.Models;
using Kaffee.Settings;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Kaffee.Services
{
    public class CoffeeService
    {
        private readonly IMongoCollection<Coffee> _coffees;

        public CoffeeService(IKaffeeDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName, new MongoDatabaseSettings {
            
            });

            _coffees = database.GetCollection<Coffee>(settings.CoffeeCollectionName);
        }

        public List<Coffee> Get(string userId) =>
            _coffees.Find(coffee => coffee.UserId.Equals(userId)).ToList();

        public Coffee GetWithId(string id) =>
            _coffees.Find<Coffee>(coffee => coffee.Id == id).FirstOrDefault();

        public Coffee Create(Coffee coffee)
        {
            _coffees.InsertOne(coffee);
            return coffee;
        }
        public void Update(string id, Coffee coffeeIn) =>
            _coffees.ReplaceOne(coffee => coffee.Id == id, coffeeIn);

        public void Remove(Coffee coffeeIn) =>
            _coffees.DeleteOne(coffee => coffee.Id == coffeeIn.Id);

        public void Remove(string id) => 
            _coffees.DeleteOne(coffee => coffee.Id == id);
    }
}