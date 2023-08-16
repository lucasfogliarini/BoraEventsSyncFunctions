using BoraEventsSyncFunctions.MinhaEntrada;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions.EventSync
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
		public async Task<IEnumerable<EventCreated>> RunAsync([TimerTrigger("%CrawlerCron%", RunOnStartup = false)] TimerInfo timer)
        {
			_logger.LogWarning("Criando eventos do Minha Entrada.");

			DateTime startDate = DateTime.Today;
			DateTime endDate = DateTime.Today.AddDays(7);

			var events = await _minhaEntradaCrawler.CrawlEventsAsync(startDate, endDate);

			return events.Select(e => new EventCreated
			{
				Start = e.DateTime,
				Title = e.Title,
				EventLink = e.EventLink,
				ImageUrl = e.ImageUrl,
				Location = e.Location,
				Public = true
			});
		}
    }
}
