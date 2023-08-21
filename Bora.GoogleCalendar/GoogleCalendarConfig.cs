using Google.Apis.Auth.OAuth2;

namespace Bora.GoogleCalendar
{
	public class GoogleCalendarConfig
	{
		public string? ClientId { get; set; }
		public string? ClientSecret { get; set; }
		public string? TokenFolder { get; set; }
		public string? ApplicationName { get; set; }
        public string? AzureStorageConnectionString { get; set; }
		public string? AzureStorageContainerName { get; set; }
		public ClientSecrets GoogleClientSecrets()
		{
			return new ClientSecrets
			{
				ClientId = ClientId,
				ClientSecret = ClientSecret
			};
		}
	}
}
