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

		public static List<PlayerRosterState> LatestActiveRosterFor(
			string teamAbbr,
			NflVerseRosterState s) =>

			s.RosterRecords
				.Where(p => p.Week == MaxWeek(s) && p.Team == teamAbbr)
				.Where(p => p.Status == "ACT")
				.OrderBy(p=> p.Position)
				.ToList();

		public static List<PlayerRosterState> LatestRosterFor(
			string teamAbbr,
			NflVerseRosterState s) =>

			s.RosterRecords
				.Where(p => p.Week == MaxWeek(s) && p.Team == teamAbbr)
				.OrderBy(p => p.Position)
				.ToList();
	}
}
