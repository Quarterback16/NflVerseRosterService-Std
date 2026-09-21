namespace NflVerseRosterService.Tests
{
	using NflVerseRosterService_Std;

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
		
			new NflVerseRosterService();
		

		[TestMethod]
		public void ServiceCanLoadState()
		{
			var result = sut.LoadRosterData();
			Assert.IsTrue(result);
		}
	}
}
