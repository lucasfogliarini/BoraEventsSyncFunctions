using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Options;

namespace BoraEventsSyncFunctions.GoogleCalendar
{
	public class AccountDataStore : IDataStore
	{
		private readonly GoogleCalendarConfig _googleCalendarConfig;

		public AccountDataStore(IOptions<GoogleCalendarConfig> googleCalendarConfig)
		{
			_googleCalendarConfig = googleCalendarConfig.Value;
		}

		public Task ClearAsync()
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync<T>(string key)
		{
			throw new NotImplementedException();
		}

		public Task<T> GetAsync<T>(string key)
		{
			throw new NotImplementedException();
		}

		public async Task<UserCredential> GetUserCredentialAsync()
		{
			UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
					_googleCalendarConfig.GoogleClientSecrets(),
					new[] { CalendarService.Scope.CalendarEvents },
					_googleCalendarConfig.Email,
					CancellationToken.None,
					this);

			return credential;
		}

		public Task StoreAsync<T>(string key, T value)
		{
			throw new NotImplementedException();
		}
	}
}
