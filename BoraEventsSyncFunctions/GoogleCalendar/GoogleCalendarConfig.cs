using Google.Apis.Auth.OAuth2;

namespace BoraEventsSyncFunctions.GoogleCalendar
{
	public class GoogleCalendarConfig
	{
		public string? ClientId { get; set; }
		public string? ClientSecret { get; set; }
		public string? TokenFolder { get; set; }
		public string? ApplicationName { get; set; }
		public string? Email { get; set; }

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
