using BoraCrawlers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
    public class StandUpSymplaSync : EventSync
	{
        const string EVENTS_QUERY = "porto-alegre-rs/stand-up-comedy";

		public StandUpSymplaSync(ILoggerFactory loggerFactory) : base(new SymplaCrawler(EVENTS_QUERY))
        {
            _logger = loggerFactory.CreateLogger<StandUpSymplaSync>();
		}

        [Function(nameof(StandUpSymplaSync))]
		[ServiceBusOutput("%AzureServiceBusEventCreatedQueue%", Connection = "AzureServiceBusConnectionString")]
		public override async Task<IEnumerable<EventCreated>> RunAsync([TimerTrigger("%CrawlerCron%", RunOnStartup = false)] TimerInfo timer)
        {
			DateTime startDate = DateTime.Today;
			DateTime endDate = DateTime.Today.AddDays(7);

			InitLog(startDate, endDate);

			var events = await _boraCrawler.CrawlEventsAsync();

			LogEvents(events);

			return events
				.Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
				.Select(e => new EventCreated
				{
					Start = e.DateTime,
					Title = e.Title,
					EventLink = e.EventLink,
					Location = e.Location
				});
		}
    }
}
