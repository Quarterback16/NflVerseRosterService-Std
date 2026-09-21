using System.Collections.Generic;
using System.Linq;

namespace NflVerseRosterService_Std
{
	public class NflVerseRosterState
	{
		public List<PlayerRosterState> RosterRecords { get; set; } 
			= new List<PlayerRosterState>();

		public int MaxWeek() => RosterRecords.Max(p => p.Week);
		public int MinWeek() => RosterRecords.Min(p => p.Week);

	}
}
