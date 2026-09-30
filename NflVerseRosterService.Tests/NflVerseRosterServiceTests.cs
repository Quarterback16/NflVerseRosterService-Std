namespace NflVerseRosterService.Tests
{
	using NflVerseRosterService_Std;
	using System;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

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
			Console.WriteLine($"Error: {result.Error}");
		}

		[TestMethod]
		public void ServiceLoadsDataWhenCorrectCsvFileNameGiven()
		{
			var result = sut.LoadRosterData("d:/dropbox/CSV/roster_2026.csv");
			Assert.IsTrue(result.IsSuccess);
			Console.WriteLine(
				$"Loaded {result.Value.RosterRecords.Count} roster records.");
			Console.WriteLine(
				$"Min Week: {NflVerseRosterHelper.MinWeek(result.Value)}");
			Console.WriteLine(
				$"Max Week: {NflVerseRosterHelper.MaxWeek(result.Value)}");
			NflVerseRosterHelper.DistinctTeams(result.Value)
				.ForEach(team => Console.WriteLine($"Team: {team}"));
		}

		[TestMethod]
		public void CanGetCurrentRosterForTeam()
		{
			Assert.IsNotNull(sut);
			var roster = sut.LatestActiveRosterFor("SF");
			roster
				.ForEach(
					player => Console.WriteLine(
						$"{player.FullName,-20} {player.Position,-2} {player.Status}"));
			Console.WriteLine();
			Console.WriteLine($"Total Active Players: {roster.Count}");
		}

		[TestMethod]
		public void CanGetTflRosterForTeam()
		{
			var nflRoster = sut.LatestRosterFor("SF");
			var tflRoster = sut.GetTflRoster("SF");
			Assert.IsNotEmpty(tflRoster);
			Console.WriteLine($"Total TFL Players: {tflRoster.Count}");
			foreach (var player in tflRoster)
			{
				var nflPlayer = nflRoster
					.FirstOrDefault(p => p.FullName == player.FullName);
				if (nflPlayer != null)
				{
					Console.WriteLine(
						$"{player.FullName,-20} {player.Position,-2} {nflPlayer.Status}");
				}
				else
				{
					Console.WriteLine(
						$"{player.FullName,-20} (Not in NFL Roster)");
				}
			}
		}

		[TestMethod]
		public void CanGetDevelopmentalRosterForTeam()
		{
			var devRoster = sut.DevRosterFor("NO");

			Assert.IsNotEmpty(devRoster);
			Console.WriteLine($"Total Developmental Players: {devRoster.Count}");
			foreach (var player in devRoster)
			{
				Console.WriteLine(
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
				player => Console.WriteLine(
					$"{player.FullName,-20} {player.Position,-2} tm: {sut.PlaysFor(player.FullName)}"));
		}

		[TestMethod]
		public void CanDetermineTeamForPlayer()
		{
			var playerName = "Deebo Samuel";
			var expectedTeamCode = "SF";
			var teamCode = sut.PlaysFor(playerName);
			Console.WriteLine($"{playerName} plays for {teamCode}");
			Assert.AreEqual(expectedTeamCode, teamCode);
		}

		[TestMethod]
		public void KnowsDistinctTeams()
		{
			var teams = NflVerseRosterHelper.DistinctTeams(
				sut.NflVerseRosters);
			Assert.IsNotEmpty(teams);
			teams.ForEach(team => Console.WriteLine($"Team: {team}"));
		}

		[TestMethod]
		public void KnowsDistinctPositions()
		{
			var positions = NflVerseRosterHelper.DistinctPositions(
				sut.NflVerseRosters);
			Assert.IsNotEmpty(positions);
			positions.ForEach(position => Console.WriteLine($"Position: {position}"));
		}

		[TestMethod]
		public void KnowsPossibleNewbies()
		{
			//  Newbies are players who have never registered a TFL stat yet, 
			//  they will be in the nflverse data but not in the TFL data.
			var nflRoster = sut.LatestRosterFor("SF")
				.Where(p => NflVerseRosterHelper.IsFantasyRelevant(p));
			Console.WriteLine($"Total NFL Fantasy Players: {nflRoster.Count()}");
			var tflRoster = sut.GetTflRoster("SF");
			Assert.IsNotEmpty(tflRoster);
			Console.WriteLine($"Total TFL Players: {tflRoster.Count}");
			Console.WriteLine("Players in NFL Roster but not in TFL Roster:");
			foreach (var player in nflRoster)
			{
				var tflPlayer = tflRoster
					.FirstOrDefault(p => p.FullName == player.FullName);
				if (tflPlayer == null)
				{
					Console.WriteLine(
						$"{player.FullName,-20} (Not in TFL Roster)");
				}
			}

		}
	}

}
