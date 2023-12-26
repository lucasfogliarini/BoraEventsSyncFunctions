using BoraCrawlers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
	public class AraujoViannaUhuuSync : EventSync
    {
        const string EVENTS_QUERY = "v/araujovianna-99";

        public AraujoViannaUhuuSync(ILoggerFactory loggerFactory) : base(new UhuuCrawler(EVENTS_QUERY))
        {
            _logger = loggerFactory.CreateLogger<AraujoViannaUhuuSync>();
        }

        [Function(nameof(AraujoViannaUhuuSync))]
        [ServiceBusOutput("%AzureServiceBusEventCreatedQueue%", Connection = "AzureServiceBusConnectionString")]
        public override async Task<IEnumerable<EventCreated>> RunAsync([TimerTrigger("%CrawlerCron%", RunOnStartup = false)] TimerInfo timer)
        {
			DateTime startDate = DateTime.Today;
			DateTime endDate = DateTime.Today.AddMonths(6);
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
                    Description = e.Price,
                    Location = e.Location
                });
        }
    }
}
