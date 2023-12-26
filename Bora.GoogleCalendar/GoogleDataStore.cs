using Google.Apis.Util.Store;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Runtime.Serialization.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using Google.Apis.Tasks.v1;

namespace Bora.GoogleCalendar
{
	public class GoogleDataStore : IDataStore
	{
		readonly BlobContainerClient _blobContainerClient;
		readonly GoogleCalendarConfig _googleCalendarConfig;

		public GoogleDataStore(IOptions<GoogleCalendarConfig> googleCalendarConfig)
		{
			_googleCalendarConfig = googleCalendarConfig.Value;
			BlobServiceClient blobServiceClient = new(_googleCalendarConfig.AzureStorageConnectionString);
			_blobContainerClient = blobServiceClient.GetBlobContainerClient(_googleCalendarConfig.AzureStorageContainerName);
			_blobContainerClient.CreateIfNotExists();
		}

		public async Task<UserCredential> GetUserCredentialAsync(string gmail)
		{
			UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
					_googleCalendarConfig.GoogleClientSecrets(),
					new[] { CalendarService.Scope.CalendarEvents, TasksService.Scope.Tasks },
					gmail,
					CancellationToken.None,
					this);

			return credential;
		}

		public async Task StoreAsync<TTokenResponse>(string gmail, TTokenResponse tokenResponse)
		{
			BlobClient blob = _blobContainerClient.GetBlobClient(gmail);
			using MemoryStream stream = new();
			new DataContractJsonSerializer(typeof(TTokenResponse)).WriteObject(stream, tokenResponse);
			stream.Position = 0;
			await blob.UploadAsync(stream, true);
		}

		public async Task DeleteAsync<T>(string key)
		{
			BlobClient blob = _blobContainerClient.GetBlobClient(key);
			await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
		}

		public async Task<TTokenResponse> GetAsync<TTokenResponse>(string gmail)
		{
			var blobName = $"{gmail}.json";
			BlobClient blob = _blobContainerClient.GetBlobClient(blobName);
			if (!await blob.ExistsAsync())
			{
				throw new ValidationException($"Calendário não autorizado ({gmail}).");
			}

			//alternative
			//var blobContent = await blobClient.DownloadContentAsync();
			//var tokenResponse = blobContent.Value.Content.ToObjectFromJson<TTokenResponse>();
			//return (TTokenResponse)tokenResponse;

			BlobDownloadInfo download = await blob.DownloadAsync();
			using MemoryStream stream = new();
			await download.Content.CopyToAsync(stream);
			stream.Position = 0;
			return (TTokenResponse)new DataContractJsonSerializer(typeof(TTokenResponse)).ReadObject(stream)!;
		}

		public async Task ClearAsync()
		{
			await foreach (BlobItem blobItem in _blobContainerClient.GetBlobsAsync())
			{
				BlobClient blob = _blobContainerClient.GetBlobClient(blobItem.Name);
				await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
			}
		}
	}
}

