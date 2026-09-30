namespace NflVerseRosterService.Tests
{
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
		
			new("d:/dropbox/CSV/roster_2026.csv");
		

		[TestMethod]
		public void ServiceChecksCorrectCsvFileName()
		{
			var result = sut.LoadRosterData("test.csv");
			Assert.IsTrue(result.IsFailure);
			WriteLine($"Error: {result.Error}");
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
			Assert.IsNotNull(sut);
			var roster = sut.LatestActiveRosterFor("SF");
			roster
				.ForEach(
					player => WriteLine(
						$"{player.FullName,-20} {player.Position,-2} {player.Status}"));
			WriteLine();
			WriteLine( $"Total Active Players: {roster.Count}");
		}

		[TestMethod]
		public void CanGetTflRosterForTeam()
		{
			var nflRoster = sut.LatestRosterFor("SF");
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

		[TestMethod]
		public void CanGetDevelopmentalRosterForTeam()
		{
			var devRoster = sut.DevRosterFor("NO");

			Assert.IsNotEmpty(devRoster);
			WriteLine($"Total Developmental Players: {devRoster.Count}");
			foreach (var player in devRoster)
			{
				WriteLine(
					$"{player.FullName,-20} {player.Position,-2} {player.Status}");
			}
		}

		[TestMethod]
		public void CanGetFalselyRosteredPlayersTeam()
		{
			var teamInFocus = "PS";
			var tflRoster = sut.GetTflRoster(teamInFocus);
			Assert.IsNotEmpty(tflRoster);
			var falslyRostered = sut.FalselyRosteredFor(
				teamInFocus, 
				tflRoster);
			falslyRostered.ForEach(
				player => WriteLine(
					$"{player.FullName,-20} {player.Position,-2} tm: {sut.PlaysFor(player.FullName)}"));
		}

		[TestMethod]
		public void CanDetermineTeamForPlayer()
		{
			var playerName = "Deebo Samuel";
			var expectedTeamCode = "SF";
			var teamCode = sut.PlaysFor(playerName);
			WriteLine($"{playerName} plays for {teamCode}");
			Assert.AreEqual(expectedTeamCode, teamCode);
		}

		[TestMethod]
		public void KnowsDistinctTeams()
		{
			var teams = NflVerseRosterHelper.DistinctTeams(
				sut.NflVerseRosters);
			Assert.IsNotEmpty(teams);
			teams.ForEach(team => WriteLine($"Team: {team}"));
		}

		[TestMethod]
		public void KnowsDistinctPositions()
		{
			var positions = NflVerseRosterHelper.DistinctPositions(
				sut.NflVerseRosters);
			Assert.IsNotEmpty(positions);
			positions.ForEach(position => WriteLine($"Position: {position}"));
		}

		[TestMethod]
		public void KnowsPossibleNewbies()
		{
			//  Newbies are players who have never registered a TFL stat yet, 
			//  they will be in the nflverse data but not in the TFL data.
			var nflRoster = sut.LatestRosterFor("SF")
				.Where(p => NflVerseRosterHelper.IsFantasyRelevant(p));
			WriteLine($"Total NFL Fantasy Players: {nflRoster.Count()}");
			var tflRoster = sut.GetTflRoster("SF");
			Assert.IsNotEmpty(tflRoster);
			WriteLine($"Total TFL Players: {tflRoster.Count}");
			WriteLine("Players in NFL Roster but not in TFL Roster:");
			foreach (var player in nflRoster)
			{
				var tflPlayer = tflRoster
					.FirstOrDefault(p => p.FullName == player.FullName);
				if (tflPlayer == null)
				{
					WriteLine(
						$"{player.FullName,-20} (Not in TFL Roster)");
				}
			}

		}
	}

}
