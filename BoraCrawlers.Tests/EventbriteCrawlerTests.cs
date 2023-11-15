namespace BoraCrawlers.Tests
{
	public class EventbriteCrawlerTests
	{
		const string ORGANIZER = "valen-bar-24177627927";

		[Fact]
		public async Task CrawlEventsAsync_Should_Return_Events()
		{
			var eventBriteCrawler = new EventbriteCrawler(ORGANIZER);

			DateTime startDate = DateTime.Today;

			var events = await eventBriteCrawler.CrawlEventsAsync();

			Assert.NotEmpty(events);
			var e = events.First(e=>e.DateTime >= startDate);

			Assert.True(e.DateTime >= startDate);
			Assert.False(string.IsNullOrEmpty(e.EventLink));
			Assert.False(string.IsNullOrEmpty(e.Title));
			Assert.False(string.IsNullOrEmpty(e.Location));
		}
	}
}