using BoraEventsSyncFunctions.Crawlers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
	public class ValenEventbriteSync : EventSync
    {
        const string ORGANIZER = "valen-bar-24177627927";

        public ValenEventbriteSync(ILoggerFactory loggerFactory) : base(new EventbriteCrawler(ORGANIZER))
        {
            _logger = loggerFactory.CreateLogger<ValenEventbriteSync>();
        }

        [Function(nameof(ValenEventbriteSync))]
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
                    Description = e.Price,
                    Location = e.Location
                });
        }
    }
}
