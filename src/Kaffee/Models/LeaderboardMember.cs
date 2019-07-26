using System.Linq;
namespace Kaffee.Models
{
    /// <summary>
    /// Model a leaderboard member.
    /// </summary>
    public class LeaderboardMember 
    {
        public string DisplayName { get; set; }
        public Coffee[] Coffees { get; set; }
        public int CoffeeCount { get => Coffees != null ? Coffees.Length : 0; }
    }
}