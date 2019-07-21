using Kaffee.Models;
using Kaffee.Settings;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kaffee.Services
{
    public class LeaderboadService
    {
        private readonly IMongoCollection<Leaderboard> _leaderboards;

        public LeaderboadService(IKaffeeDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName, new MongoDatabaseSettings());

            _leaderboards = database.GetCollection<Leaderboard>(settings.LeaderboardCollectionName);
        }

        public List<Leaderboard> GetUsersLeaderboards(string userId) =>
            _leaderboards.Find(
                board => board.Members.Contains(userId) || 
                    board.Administrators.Contains(userId)
            ).ToList();

        public Leaderboard Get(string id) =>
            _leaderboards.Find<Leaderboard>(board => board.Id == id).FirstOrDefault();

        public async Task<Leaderboard> Create(Leaderboard board)
        {
            await _leaderboards.InsertOneAsync(board);
            return board;
        }
        public async Task Update(string id, Leaderboard boardIn) =>
            await _leaderboards.ReplaceOneAsync(board => board.Id == id, boardIn);

        public void Remove(Leaderboard boardIn) =>
            _leaderboards.DeleteOne(board => board.Id == boardIn.Id);

        public void Remove(string id) => 
            _leaderboards.DeleteOne(board => board.Id == id);
    }
}