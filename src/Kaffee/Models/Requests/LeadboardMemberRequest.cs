namespace Kaffee.Models.Requests
{
    /// <summary>
    /// Model a leaderboard member request.
    /// </summary>
    public class LeaderboardMemberRequest
    {
        public string BoardId { get; set; }
        public string Email { get; set; }
    }
}