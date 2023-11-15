namespace BoraCrawlers.Tests
{
	public class MinhaEntradaCrawlerTests
	{
		const string EVENTS_QUERY = "?estado=RS&cidade=7994&categoria=4";

		[Fact]
		public async Task CrawlEventsAsync_Should_Return_Events()
		{
			var minhaEntradaCrawler = new MinhaEntradaCrawler(EVENTS_QUERY);

			DateTime startDate = DateTime.Today;
			DateTime endDate = DateTime.Today.AddDays(7);

			minhaEntradaCrawler.EventsSchedule = $"{minhaEntradaCrawler.EventsSchedule}&data-inicio={startDate:yyyy-MM-dd}&data-fim={endDate:yyyy-MM-dd}";

			var events = await minhaEntradaCrawler.CrawlEventsAsync();

			Assert.NotEmpty(events);
			var e = events.First();

			Assert.True(e.DateTime >= startDate);
			Assert.False(string.IsNullOrEmpty(e.EventLink));
			Assert.False(string.IsNullOrEmpty(e.Title));
			Assert.False(string.IsNullOrEmpty(e.Location));
		}
	}
}