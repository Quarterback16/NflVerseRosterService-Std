using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using NameFixerService;
using PlayerService_2._0;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NflVerseRosterService_Std
{
	public class NflVerseRosterService
	{
		private readonly INflPlayerService _ps;
		private readonly IFixNames _nameFixer;

		public NflVerseRosterService()
		{
			_ps = new NflPlayerService();
			_nameFixer = new NameFixer();
		}

		public Result<NflVerseRosterState> LoadRosterData(
			string csvFile)
		{
			// Implement the logic to load roster data here
			if (!File.Exists(csvFile))
			{
				return Result.Failure<NflVerseRosterState>(
					$"CSV file not found: {csvFile}");
			}
			var rosterRecords = ReadCsv(csvFile);
			var rosterState = new NflVerseRosterState
			{
				RosterRecords = rosterRecords
			};
			return Result.Success(rosterState); // Placeholder return value
		}

		private List<PlayerRosterState> ReadCsv(
			string inputFile)
		{
			var retval = new List<PlayerRosterState>();
			var config = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = true,
				HeaderValidated = null,
				MissingFieldFound = null,
				// Normalize headers to lowercase for matching
				PrepareHeaderForMatch = args => args.Header.ToLower()
			};
			var classMap = new NflVerseRosterCsvInputClassMap2(_nameFixer);

			using (var reader = new StreamReader(inputFile))
			using (var csv = new CsvReader(reader, config))
			{
				csv.Context.RegisterClassMap(classMap);
				retval.AddRange(csv.GetRecords<PlayerRosterState>());
			}
			return retval;
		}

		public List<PlayerRosterState> GetTflRoster(string teamCode)
		{
			var result = new List<PlayerRosterState>();
			var players = _ps.Search(p => p.CurrTeam == teamCode);
			foreach (var player in players)
			{
				var rosterState = new PlayerRosterState
				{
					Team = player.CurrTeam,

					Position = player.Pos,
					JerseyNumber = JerseyNumber(player.JerseyNo),
					Status = "ACT",
					FullName = player.Name,
				};
				result.Add(rosterState);
			}
			return result;
		}

		private static int? JerseyNumber(string jerseyNo)
		{
			if (string.IsNullOrEmpty(jerseyNo))
			{
				return null;
			}
			return int.Parse(jerseyNo);
		}
	}
}
