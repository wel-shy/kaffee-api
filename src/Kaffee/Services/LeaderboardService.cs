using System;
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
        private readonly UserService _userService;

        public LeaderboadService(IKaffeeDatabaseSettings settings, UserService _userService)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName, new MongoDatabaseSettings());

            this._userService = _userService;

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

        public async Task<Leaderboard> AddMember(Leaderboard board, string email)
        {
            var id = await GetUserId(email);

            var members = board.Members.ToList();
            members.Add(id);
            board.Members = members.ToArray();

            await Update(board.Id, board);

            return board;
        }

        public async Task<Leaderboard> RemoveMember(Leaderboard board, string email)
        {
            var id = await GetUserId(email);

            var members = board.Members.ToList();
            members.Remove(id);
            board.Members = members.ToArray();

            await Update(board.Id, board);

            return board;
        }

        public async Task<Leaderboard> AddAdmin(Leaderboard board, string email)
        {
            var id = await GetUserId(email);

            var admins = board.Administrators.ToList();
            admins.Add(id);
            board.Members = admins.ToArray();

            await Update(board.Id, board);

            return board;
        }

        public async Task<Leaderboard> RemoveAdmin(Leaderboard board, string email)
        {
            var id = await GetUserId(email);

            var admins = board.Administrators.ToList();
            admins.Remove(id);
            board.Members = admins.ToArray();

            await Update(board.Id, board);

            return board;
        }

        public async Task Update(string id, Leaderboard boardIn) =>
            await _leaderboards.ReplaceOneAsync(board => board.Id == id, boardIn);

        public void Remove(Leaderboard boardIn) =>
            _leaderboards.DeleteOne(board => board.Id == boardIn.Id);

        public void Remove(string id) =>
            _leaderboards.DeleteOne(board => board.Id == id);

        private async Task<string> GetUserId(string email) 
        {
            var user = await _userService.GetWithEmail(email);
            if(user == null)
            {
                throw new Exception("User not found.");
            }

            return user.Id;
        }
    }
}