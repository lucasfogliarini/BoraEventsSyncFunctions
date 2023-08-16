using BoraEventsSyncFunctions.Eventbrite;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
    public class EventbriteSync
    {
        private readonly ILogger _logger;
        const string ORGANIZER = "valen-bar-24177627927";
        readonly EventbriteCrawler _eventbriteCrawler;

        public EventbriteSync(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EventbriteSync>();
            _eventbriteCrawler = new EventbriteCrawler(ORGANIZER);
        }

        [Function(nameof(EventbriteSync))]
        [ServiceBusOutput("%AzureServiceBusEventCreatedQueue%", Connection = "AzureServiceBusConnectionString")]
        public async Task<IEnumerable<EventCreated>> RunAsync([TimerTrigger("%CrawlerCron%", RunOnStartup = false)] TimerInfo timer)
        {
			_logger.LogWarning("Criando eventos do Eventbrite.");

			DateTime startDate = DateTime.Today;
			DateTime endDate = DateTime.Today.AddDays(7);

			var events = await _eventbriteCrawler.CrawlEventsAsync();

            return events
                .Where(e => e.DateTime >= startDate && e.DateTime <= endDate)
                .Select(e => new EventCreated
                {
                    Start = e.DateTime,
                    Title = e.Title,
                    EventLink = e.EventLink,
                    Description = e.Price,
                    Location = e.Location,
                    Public = true
                });
        }
    }
}
