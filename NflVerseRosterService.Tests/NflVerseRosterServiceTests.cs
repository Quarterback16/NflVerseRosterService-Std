namespace NflVerseRosterService.Tests
{
	using CSharpFunctionalExtensions;
	using NflVerseRosterService_Std;
	using static System.Console;

	[TestClass]
	public class NflVerseRosterServiceTests
	{
		private NflVerseRosterService sut;

		[TestInitialize]
		public void Setup()
		{
			sut = ServiceUnderTest();
		}

		private static NflVerseRosterService ServiceUnderTest() =>
		
			new();
		

		[TestMethod]
		public void ServiceChecksCorrectCsvFileName()
		{
			var result = sut.LoadRosterData("test.csv");
			Assert.IsTrue(result.IsFailure);
		}

		[TestMethod]
		public void ServiceLoadsDataWhenCorrectCsvFileNameGiven()
		{
			var result = sut.LoadRosterData("d:/dropbox/CSV/roster_2026.csv");
			Assert.IsTrue(result.IsSuccess);
			WriteLine(
				$"Loaded {result.Value.RosterRecords.Count} roster records.");
			WriteLine(
				$"Min Week: {NflVerseRosterHelper.MinWeek(result.Value)}");
			WriteLine(
				$"Max Week: {NflVerseRosterHelper.MaxWeek(result.Value)}");
			NflVerseRosterHelper.DistinctTeams(result.Value)
				.ForEach(team => WriteLine($"Team: {team}"));
		}

		[TestMethod]
		public void CanGetCurrentRosterForTeam()
		{
			var result = sut.LoadRosterData("d:/dropbox/CSV/roster_2026.csv");
			Assert.IsTrue(result.IsSuccess);
			NflVerseRosterHelper.LatestActiveRosterFor("SF", result.Value)
				.ForEach(
					player => WriteLine(
						$"{player.FullName,-20} {player.Position,-2} {player.Status}"));
			WriteLine();
			WriteLine( $"Total Active Players: {NflVerseRosterHelper.LatestActiveRosterFor("SF", result.Value).Count}");
		}

		[TestMethod]
		public void CanGetTflRosterForTeam()
		{
			var nflRosterData = sut.LoadRosterData("d:/dropbox/CSV/roster_2026.csv");
			Assert.IsNotEmpty(nflRosterData.Value.RosterRecords);
			var nflRoster = NflVerseRosterHelper.LatestRosterFor("SF", nflRosterData.Value);
			var tflRoster = sut.GetTflRoster("SF");
			Assert.IsNotEmpty(tflRoster);
			WriteLine($"Total TFL Players: {tflRoster.Count}");
			foreach (var player in tflRoster)
			{
				var nflPlayer = nflRoster
					.FirstOrDefault(p => p.FullName == player.FullName);
				if (nflPlayer != null)
				{
					WriteLine(
						$"{player.FullName,-20} {player.Position,-2} {nflPlayer.Status}");
				}
				else
				{
					WriteLine(
						$"{player.FullName,-20} (Not in NFL Roster)");
				}
			}
		}
	}
}
