using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NflVerseRosterService_Std
{
	public class NflVerseRosterService
	{
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

		private static List<PlayerRosterState> ReadCsv(
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
			var classMap = new NflVerseRosterCsvInputClassMap2();

			using (var reader = new StreamReader(inputFile))
			using (var csv = new CsvReader(reader, config))
			{
				csv.Context.RegisterClassMap(classMap);
				retval.AddRange(csv.GetRecords<PlayerRosterState>());
			}
			return retval;
		}
	}
}
