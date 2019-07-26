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
        private readonly CoffeeService _coffeeService;

        /// <summary>
        /// Get an instance of a leaderboard service.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="_userService"></param>
        /// <param name="_coffeeService"></param>
        public LeaderboadService(
            IKaffeeDatabaseSettings settings, 
            UserService _userService,
            CoffeeService _coffeeService
        )
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName, new MongoDatabaseSettings());

            this._userService = _userService;
            this._coffeeService = _coffeeService;

            _leaderboards = database.GetCollection<Leaderboard>(settings.LeaderboardCollectionName);
        }

        /// <summary>
        /// Get a users leaderboards
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Leaderboard> GetUsersLeaderboards(string userId) =>
            _leaderboards.Find(
                board => board.Members.Contains(userId) ||
                    board.Administrators.Contains(userId)
            ).ToList();

        public Leaderboard Get(string id) =>
            _leaderboards.Find<Leaderboard>(board => board.Id == id).FirstOrDefault();

        /// <summary>
        /// Create a leaderboard.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public async Task<Leaderboard> Create(Leaderboard board)
        {
            // Assign random colour if not set.
            if (string.IsNullOrEmpty(board.Colour))
            {
                board.Colour = String.Format("#{0:X6}", new Random().Next(0x1000000));
            }
            await _leaderboards.InsertOneAsync(board);
            return board;
        }

        /// <summary>
        /// Add a member to a leaderboard.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Leaderboard> AddMember(Leaderboard board, string email)
        {
            var id = await GetUserId(email);

            var members = board.Members.ToList();
            members.Add(id);
            board.Members = members.ToArray();

            await Update(board.Id, board);

            return board;
        }

        /// <summary>
        /// Remove a member from a leaderboard.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Leaderboard> RemoveMember(Leaderboard board, string email)
        {
            var id = await GetUserId(email);

            var members = board.Members.ToList();
            members.Remove(id);
            board.Members = members.ToArray();

            await Update(board.Id, board);

            return board;
        }

        /// <summary>
        /// Add an admin to a leaderboard.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Leaderboard> AddAdmin(Leaderboard board, string email)
        {
            var id = await GetUserId(email);

            var admins = board.Administrators.ToList();
            admins.Add(id);
            board.Members = admins.ToArray();

            await Update(board.Id, board);

            return board;
        }

        /// <summary>
        /// Remove an admin from a leaderboard.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Leaderboard> RemoveAdmin(Leaderboard board, string email)
        {
            var id = await GetUserId(email);

            var admins = board.Administrators.ToList();
            admins.Remove(id);
            board.Members = admins.ToArray();

            await Update(board.Id, board);

            return board;
        }

        /// <summary>
        /// Update a leaderboard.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="boardIn"></param>
        /// <returns></returns>
        public async Task Update(string id, Leaderboard boardIn) =>
            await _leaderboards.ReplaceOneAsync(board => board.Id == id, boardIn);

        public void Remove(Leaderboard boardIn) =>
            _leaderboards.DeleteOne(board => board.Id == boardIn.Id);
        
        /// <summary>
        /// Get all members in a leaderboard.
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public LeaderboardMember[] GetLeaderboardMembers(Leaderboard board)
        {
            List<LeaderboardMember> members = new List<LeaderboardMember>();
            string[] memberIds = board.Administrators
                .Concat(board.Members)
                .ToHashSet()
                .ToArray();

            foreach (var member in memberIds)
            {
                members.Add(GetLeaderboardMember(member, board));
            }

            return members.ToArray();
        }

        /// <summary>
        /// Get the full details of a leaderboard member.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="board"></param>
        /// <returns></returns>
        public LeaderboardMember GetLeaderboardMember(string id, Leaderboard board) 
        {
            var user = _userService.Get(id);
            var coffees = _coffeeService.GetFromDate(user.Id, board.CreatedAt);
            LeaderboardMember member = new LeaderboardMember
            {
                DisplayName = user.DisplayName ?? user.Email,
                Coffees = coffees.ToArray()
            };

            return member;
        }

        /// <summary>
        /// Delete a leaderboard.
        /// </summary>
        /// <param name="id"></param>
        public void Remove(string id) =>
            _leaderboards.DeleteOne(board => board.Id == id);

        /// <summary>
        /// Get a userid from an email address.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
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