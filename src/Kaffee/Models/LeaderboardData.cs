namespace Kaffee.Models
{
    /// <summary>
    /// Leaderboard data for when full details of a board is fetched.
    /// </summary>
    public class LeaderboardData 
    {
        public Leaderboard Leaderboard { get; set; }
        public LeaderboardMember[] Members { get; set; }
    }
}