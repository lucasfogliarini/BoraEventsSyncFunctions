using BoraEventsSyncFunctions.GoogleCalendar;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions
{
	public class GoogleCalendarSync
	{
		readonly ILogger _logger;
		readonly GoogleCalendarService _googleCalendarService;

		public GoogleCalendarSync(GoogleCalendarService googleCalendarService, ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<GoogleCalendarSync>();
			_googleCalendarService = googleCalendarService;
		}

		[Function(nameof(GoogleCalendarSync))]
		public async Task Run([ServiceBusTrigger("%AzureServiceBusEventCreatedQueue%",
											Connection = "AzureServiceBusConnectionString")]
											EventCreated eventCreated)
		{
			_logger.LogWarning("Sincronizando evento com o Google Calendar.");
			//await _googleCalendarService.CreateAsync(eventCreated);
		}
	}
}
