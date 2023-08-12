using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions
{
	public class GoogleCalendarSync
	{
		readonly ILogger _logger;
		readonly BoraHttpClient _boraHttpClient = new();

		public GoogleCalendarSync(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<GoogleCalendarSync>();
		}

		[Function(nameof(GoogleCalendarSync))]
		public async Task Run([ServiceBusTrigger("%AzureServiceBusEventCreatedQueue%",
											Connection = "AzureServiceBusConnectionString")]
											EventCreated eventCreated)
		{
			_logger.LogWarning("Sincronizando evento com o Google Calendar.");
			eventCreated.Description = eventCreated.EventLink;
			await _boraHttpClient.PostEventAsync(eventCreated);
		}
	}
}
