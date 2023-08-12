using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;

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
											Event eventsCreated)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Event Details:");
			sb.AppendLine($"Title: {eventsCreated.Title}");
			sb.AppendLine($"DateTime: {eventsCreated.DateTime}");
			sb.AppendLine($"Location: {eventsCreated.Location}");
			sb.AppendLine($"ImageUrl: {eventsCreated.ImageUrl}");
			sb.AppendLine($"EventLink: {eventsCreated.EventLink}");

			_logger.LogInformation(sb.ToString());
		}
	}
}
