using BoraCrawlers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
{
	public class ValenEventbriteSync : EventSync<ValenEventbriteSync>
    {
		const string ORGANIZER_ID = "24177627927";
		private readonly EventbriteHttpClient _eventbriteHttpClient;

		public ValenEventbriteSync(ILoggerFactory loggerFactory, EventbriteHttpClient eventbriteHttpClient) : base(loggerFactory)
        {
            _eventbriteHttpClient = eventbriteHttpClient;
		}

        [Function(nameof(ValenEventbriteSync))]
        [ServiceBusOutput("%AzureServiceBusEventCreatedQueue%", Connection = "AzureServiceBusConnectionString")]
        public override async Task<IEnumerable<EventCreated>> RunAsync([TimerTrigger("%CrawlerCron%", RunOnStartup = false)] TimerInfo timer)
        {
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(7);

            var eventsScheduleUrl = "https://www.eventbrite.com.br/o/valen-bar-24177627927/";

			InitLog(startDate, endDate, eventsScheduleUrl);

            var eventbriteEvents = await _eventbriteHttpClient.ListEventsAsync(ORGANIZER_ID);

            var events = eventbriteEvents
                .Where(e => e.Start.Local >= startDate && e.Start.Local <= endDate)
                .Select(e => new EventCreated
                {
                    Start = e.Start.Local,
                    Title = ParseValenTitle(e.Name.Text),
                    EventLink = e.Url,
                    Description = e.Summary,
					Location = "❤️‍🔥 Valen"
				});

			LogEvents(events);

            return events;
        }

		string ParseValenTitle(string title)
		{
			return title.Contains("VALEN") ? title[23..] : title;
		}
	}
}
