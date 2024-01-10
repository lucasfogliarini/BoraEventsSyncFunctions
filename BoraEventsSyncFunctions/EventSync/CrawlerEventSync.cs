using BoraCrawlers;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
	public abstract class CrawlerEventSync<TEventSync> : EventSync<TEventSync>
	{
		protected BoraCrawler BoraCrawler { get; set; }
        public CrawlerEventSync(ILoggerFactory loggerFactory, BoraCrawler boraCrawler) : base(loggerFactory)
		{
			BoraCrawler = boraCrawler;			
			_logger = loggerFactory.CreateLogger<TEventSync>();
		}

		protected async Task<IEnumerable<EventCreated>> CrawlEventsAsync(DateTime startDate, DateTime endDate)
		{
			InitLog(startDate, endDate, BoraCrawler.EventsSchedule!);

			var eventsCrawled = await BoraCrawler.CrawlEventsAsync();
			var events = eventsCrawled
				.Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
				.Select(e => new EventCreated
				{
					Start = e.DateTime,
					Title = e.Title,
					EventLink = e.EventLink,
					Description = e.Price,
					Location = e.Location
				});

			LogEvents(events);

			return events;
		}
	}
}
