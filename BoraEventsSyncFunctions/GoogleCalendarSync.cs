using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BoraEventsSyncFunctions
{
	public class GoogleCalendarSync
	{
		private readonly ILogger _logger;

		public GoogleCalendarSync(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<GoogleCalendarSync>();
		}

		[Function(nameof(GoogleCalendarSync))]
		public void Run([ServiceBusTrigger("%AzureServiceBusEventCreatedQueue%",
											Connection = "AzureServiceBusConnectionString")]
											string eventCreated)
		{
			_logger.LogInformation($"C# ServiceBus queue trigger function processed message: {eventCreated}");
		}
	}
}
