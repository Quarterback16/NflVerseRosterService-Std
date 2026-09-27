using CsvHelper.Configuration;
using NameFixerService;

namespace NflVerseRosterService_Std
{
	public class NflVerseRosterCsvInputClassMap2 : ClassMap<PlayerRosterState>
	{
		public NflVerseRosterCsvInputClassMap2(IFixNames nameFixer)
		{
			ConfigureMappings(this, nameFixer);
		}

		private static void ConfigureMappings(
			ClassMap<PlayerRosterState> map,
			IFixNames nameFixer)
		{ 
			// map for stru 2026-09-21 
			// Fields must match Header Names Exactly!
			// The you need to get the order right
			map.Map(p => p.Season).Name("season");
			map.Map(p => p.Week).Name("week");
			map.Map(p => p.Team).Name("team");
			map.Map(p => p.Position).Name("position");
			map.Map(p => p.DepthChartPosition).Name("depth_chart_position");
			map.Map(p => p.JerseyNumber)
				.Name("jersey_number")
				.Optional();
			map.Map(p => p.Status).Name("status");
			map.Map(p => p.FullName)
				.Name("full_name")
				.Convert(
					args =>
					{ 
						var name = args.Row.GetField("full_name");
						return nameFixer.FixName(name);
					});
			map.Map(p => p.FirstName).Name("first_name");
			map.Map(p => p.LastName).Name("last_name");
			map.Map(p => p.BirthDate)
				.Name("birth_date")
				.TypeConverterOption.Format("yyyy-MM-dd")
				.Optional();
			map.Map(p => p.DraftNumber).Name("draft_number");
		}
	}
}