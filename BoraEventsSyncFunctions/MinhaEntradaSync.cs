using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MinhaEntrada;

namespace BoraEventsSyncFunctions
{
	public class MinhaEntradaSync
    {
        private readonly ILogger _logger;
        const string ORGANIZER = "PoaComedyClub";
        readonly MinhaEntradaCrawler _minhaEntradaCrawler;

		public MinhaEntradaSync(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MinhaEntradaSync>();
			_minhaEntradaCrawler = new MinhaEntradaCrawler(ORGANIZER);
		}

        [Function(nameof(MinhaEntradaSync))]
		[ServiceBusOutput("%AzureServiceBusEventCreatedQueue%", Connection = "AzureServiceBusConnectionString")]
		public async Task<IEnumerable<Event>> Run([TimerTrigger("%MinhaEntradaCron%")] TimerInfo timer)
        {
			DateTime startDate = DateTime.Today;
			DateTime endDate = DateTime.Today.AddDays(7);

			var events = await _minhaEntradaCrawler.CrawlEventsAsync(startDate, endDate);
			return events.Select(e => new Event
			{
				DateTime = e.DateTime,
				Title = e.Title,
				EventLink = e.EventLink,
				ImageUrl = e.ImageUrl,
				Location = e.Location
			});
		}
    }
}
