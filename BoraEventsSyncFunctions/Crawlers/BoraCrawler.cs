using AngleSharp.Dom;
using AngleSharp;
using System.Globalization;

namespace BoraEventsSyncFunctions.Crawlers
{
	public abstract class BoraCrawler
	{
        public string? EventsSchedule { get; set; }

		public async Task<IEnumerable<CrawledEvent>> CrawlEventsAsync()
		{
			var document = await GetHtmlDocumentAsync();
			IEnumerable<CrawledEvent> events = ExtractEvents(document);
			return events.OrderBy(e => e.DateTime);
		}

		protected abstract List<CrawledEvent> ExtractEvents(IDocument document);

		protected async Task<IDocument> GetHtmlDocumentAsync()
		{
			var config = Configuration.Default.WithDefaultLoader();
			var context = BrowsingContext.New(config);

			using (var httpClient = new HttpClient())
			{
				var html = await httpClient.GetStringAsync(EventsSchedule);
				return await context.OpenAsync(req => req.Content(html));
			}
		}

		protected static DateTime? ParseDateTime(string d, string MMMBr, string HHmm)
		{
			Dictionary<string, string> monthMap = new()
			{
				{ "jan", "Jan" },
				{ "fev", "Feb" },
				{ "mar", "Mar" },
				{ "abr", "Apr" },
				{ "mai", "May" },
				{ "jun", "Jun" },
				{ "jul", "Jul" },
				{ "ago", "Aug" },
				{ "set", "Sep" },
				{ "out", "Oct" },
				{ "nov", "Nov" },
				{ "dez", "Dec" }
			};

			string MMM = monthMap[MMMBr.ToLower()];
			string format = "d MMM HH:mm";
			string formattedDate = $"{d} {MMM} {HHmm}";

			if (DateTime.TryParseExact(formattedDate, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime dateTime))
			{
				return dateTime;
			}

			return null;
		}
	}
}
