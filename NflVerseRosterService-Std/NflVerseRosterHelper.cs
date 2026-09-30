using System.Collections.Generic;
using System.Linq;

namespace NflVerseRosterService_Std
{
	public static class NflVerseRosterHelper
	{
		public static int MaxWeek(NflVerseRosterState s) => 
			s.RosterRecords.Max(p => p.Week);
		public static int MinWeek(NflVerseRosterState s) => 
			s.RosterRecords.Min(p => p.Week);

		public static List<string> DistinctTeams(NflVerseRosterState s) =>
		
			s.RosterRecords
				.GroupBy(p => p.Team)
				.Select(g => g.First().Team)
				.OrderBy(x => x)
				.ToList();

		public static List<string> DistinctPositions(
			NflVerseRosterState s) =>

			s.RosterRecords
				.GroupBy(p => p.Position)
				.Select(g => g.First().Position)
				.OrderBy(x => x)
				.ToList();

		public static List<PlayerRosterState> LatestRosterFor(
			string teamAbbr,
			NflVerseRosterState s) =>

			s.RosterRecords
				.Where(p => p.Week == MaxWeek(s) && p.Team == teamAbbr)
				.OrderBy(p => p.Position)
				.ToList();

		public static List<PlayerRosterState> RosterFor(
			string teamAbbr,
			string statusCode,
			NflVerseRosterState s) =>

			s.RosterRecords
				.Where(p => p.Week == MaxWeek(s) && p.Team == teamAbbr)
				.Where(p => p.Status == statusCode)
				.OrderBy(p => p.Position)
				.ToList();

		public static List<PlayerRosterState> FalselyRosteredFor(
			string teamAbbr,
			NflVerseRosterState s,
			List<PlayerRosterState> suggestedRoster)
		{
			var falselyRostered = new List<PlayerRosterState>();
			var nflRoster = s.RosterRecords
				.Where(p => p.Week == MaxWeek(s) && p.Team == teamAbbr)
				.OrderBy(p => p.Position)
				.ToList();
			foreach (var player in suggestedRoster)
			{
				var nflPlayer = nflRoster
					.FirstOrDefault(p => p.FullName == player.FullName);
				if (nflPlayer == null)
				{
					falselyRostered.Add(player);
				}
			}
			return falselyRostered;
		}

		public static bool IsFantasyRelevant(PlayerRosterState player) =>

			player.Position == "QB" ||
			player.Position == "RB" ||
			player.Position == "WR" ||
			player.Position == "TE" ||
			player.Position == "K";


	}
}
