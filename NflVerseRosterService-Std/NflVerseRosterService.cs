using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using Helpers_Std;
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

		private readonly Dictionary<string,string> TeamCodes = new Dictionary<string,string>
		{
			{ "ARI", "AC" },
			{ "ATL", "AF" },
			{ "BAL", "BR" },
			{ "BUF", "BB" },
			{ "CAR", "CP" },
			{ "CHI", "CH" },
			{ "CIN", "CI" },
			{ "CLE", "CL" },
			{ "DAL", "DC" },
			{ "DEN", "DB" },
			{ "DET", "DL" },
			{ "GB",  "GB" },
			{ "HOU", "HT" },
			{ "IND", "IC" },
			{ "JAX", "JJ" },
			{ "KC",  "KC" },
			{ "LA",  "LR" },
			{ "LAC", "LC" },
			{ "LV",  "OR" },
			{ "MIA", "MD" },
			{ "MIN", "MV" },
			{ "NE",  "NE" },
			{ "NO",  "NO" },
			{ "NYG", "NG" },
			{ "NYJ", "NJ" },
			{ "PHI", "PE" },
			{ "PIT", "PS" },
			{ "SEA", "SS" },
			{ "SF",  "SF" },
			{ "TB",  "TB" },
			{ "TEN", "TT" },
			{ "WAS", "WR" },
		};

		public NflVerseRosterState NflVerseRosters { get; set; }
		public string CsvFilePath { get; set; }
		public int Season { get; set; }

		public NflVerseRosterService(
			int season)
		{
			Season = season;
			_ps = new NflPlayerService(
				FolderHelper.PlayerCsvFile(
					season.ToString()));
			_nameFixer = new NameFixer();
			CsvFilePath = $"{FolderHelper.CsvFolder()}roster_{season}.csv";
			LoadRosterData();
		}

		public NflVerseRosterService(
			string csvFilePath)
		{
			_ps = new NflPlayerService(
				FolderHelper.PlayerCsvFile(
					Season.ToString()));
			_nameFixer = new NameFixer();
			CsvFilePath = csvFilePath;
			LoadRosterData();
		}

		private void LoadRosterData()
		{
			var result = LoadRosterData(CsvFilePath);
			if (result.IsSuccess)
			{
				NflVerseRosters = result.Value;
				Purify();
			}
		}

		public CSharpFunctionalExtensions.Result<NflVerseRosterState> LoadRosterData(
			string csvFile)
		{
			// Implement the logic to load roster data here
			if (!File.Exists(csvFile))
			{
				return CSharpFunctionalExtensions.Result.Failure<NflVerseRosterState>(
					$"CSV file not found: {csvFile}");
			}
			var rosterRecords = ReadCsv(csvFile);
			var rosterState = new NflVerseRosterState
			{
				RosterRecords = rosterRecords
			};
			return CSharpFunctionalExtensions.Result.Success(rosterState); // Placeholder return value
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

		public string PlaysFor(
			string playerName)
		{
			var player = NflVerseRosters.RosterRecords
				.Find(p => p.Week == NflVerseRosterHelper.MaxWeek(NflVerseRosters) 
					&& p.FullName == playerName);
			return player?.Team ?? string.Empty;
		}

		private void Purify()
		{
			NflVerseRosters.RosterRecords.ForEach(p =>
			{
				p.FullName = _nameFixer.FixName(p.FullName);
				p.Team = TflCodeFor(p.Team);
			});
		}

		private string TflCodeFor(string teamCode) =>
		
			TeamCodes[teamCode];
		

		public List<PlayerRosterState> FalselyRosteredFor(
			string teamAbbr,
			List<PlayerRosterState> suggestedRoster)
		{
			var falselyRostered = NflVerseRosterHelper.FalselyRosteredFor(
				teamAbbr,
				NflVerseRosters,
				suggestedRoster);
			return falselyRostered;
		}

		public List<PlayerRosterState> LatestActiveRosterFor(
			string teamAbbr) =>
		
			NflVerseRosterHelper.RosterFor(
				teamAbbr,
				"ACT",
				NflVerseRosters);

		public List<PlayerRosterState> LatestRosterFor(
			string teamAbbr) =>

			NflVerseRosterHelper.LatestRosterFor(
				teamAbbr,
				NflVerseRosters);

		public List<PlayerRosterState> DevRosterFor(
			string teamAbbr) =>

			NflVerseRosterHelper.RosterFor(
				teamAbbr,
				"DEV",
				NflVerseRosters);

		public List<string> DistinctTeams() =>

			NflVerseRosterHelper.DistinctTeams(NflVerseRosters);

		public List<string> DistinctPositions() =>

			NflVerseRosterHelper.DistinctPositions(NflVerseRosters);
	}
}