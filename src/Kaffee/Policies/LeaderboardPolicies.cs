using System.Linq;
using Kaffee.Models;

namespace Kaffee.Policies
{
    /// <summary>
    /// Policies for limiting actions on a leaderboard.
    /// </summary>
    public class LeaderboardPolicies
    {
        /// <summary>
        /// Check if user can read a leaderboard.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool CanRead(Leaderboard board, string userId) 
        {
            var members = board.Administrators.Concat(board.Members);
            if (members.Contains(userId)) return true;

            return false;
        }

        /// <summary>
        /// Check if user can edit a leaderboard.
        /// </summary>
        /// <param name="board"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool CanEdit(Leaderboard board, string userId)
        {
            if (board.Administrators == null) return false;
            if (board.Administrators.Contains(userId)) return true;
            return false;
        }
    }
}