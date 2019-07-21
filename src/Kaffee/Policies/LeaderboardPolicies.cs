using System.Linq;
using Kaffee.Models;

namespace Kaffee.Policies
{
    public class LeaderboardPolicies
    {
        public static bool CanRead(Leaderboard board, string userId) 
        {
            var members = board.Administrators.Concat(board.Members);
            if (members.Contains(userId)) return true;

            return false;
        }

        public static bool CanEdit(Leaderboard board, string userId)
        {
            if (board.Administrators.Contains(userId)) return true;
            return false;
        }
    }
}