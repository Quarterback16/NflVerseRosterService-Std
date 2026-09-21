using CsvHelper.Configuration.Attributes;
using System;

namespace NflVerseRosterService_Std
{
	public class PlayerRosterState
	{
		[Name("season")]
		public int Season { get; set; }
		[Name("team")]
		public string Team { get; set; }
		[Name("week")]
		public int Week { get; set; }
		[Name("position")]
		public string Position { get; set; }
		[Name("depth_chart_position")]
		public string DepthChartPosition { get; set; }
		[Name("jersey_number")]
		public int? JerseyNumber { get; set; }
		[Name("status")]
		public string Status { get; set; }
		[Name("full_name")]
		public string FullName { get; set; }
		[Name("first_name")]
		public string FirstName { get; set; }
		[Name("last_name")]
		public string LastName { get; set; }
		[Name("birth_date")]
		public DateTime? BirthDate { get; set; }
		[Name("draft_number")]
		public int? DraftNumber { get; set; }
	}
}
