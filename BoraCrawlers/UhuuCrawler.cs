using AngleSharp.Dom;

namespace BoraCrawlers
{
	public class UhuuCrawler : BoraCrawler
    {
        const string UHUU_DOMAIN = "https://uhuu.com";

		/// <summary>
		/// A crawler class for extracting event information from a 'https://uhuu.com/'
		/// </summary>
		/// <param name="eventsQuery">https://uhuu.com/{eventsQuery}</param>
		public UhuuCrawler(string eventsQuery)
        {
            EventsSchedule = $"{UHUU_DOMAIN}/{eventsQuery}";
        }

        protected override List<CrawledEvent> ExtractEvents(IDocument document)
        {
            var events = document.QuerySelectorAll(".item.card-evento");

			List<CrawledEvent> eventList = new();

            foreach (var eventNode in events)
            {
				var title = eventNode.QuerySelector(".info-evento .evento-nome")?.TextContent.Trim();
				var dd = eventNode.QuerySelector(".info-evento .data-layer")?.TextContent.Trim();
                dd ??= eventNode.QuerySelector(".info-evento .data-inicial")?.TextContent.Trim();
				var MMMBr = eventNode.QuerySelector(".info-evento .data-mes")?.TextContent.Substring(0,3).Trim();
				var location = eventNode.QuerySelector(".info-evento .local-nome")?.TextContent.Trim();
				var eventLink = eventNode.QuerySelector(".link")?.GetAttribute("href");
                var imageUrl = eventNode.QuerySelector(".container-img img")?.GetAttribute("data-src");

				var crawledEvent = new CrawledEvent
                {
                    Title = title,
                    DateTime = ParseDateTime(dd, MMMBr),
                    Location = location,
                    EventLink = eventLink,
                    ImageUrl = imageUrl,
                };

                eventList.Add(crawledEvent);
            }

            return eventList;
        }
    }
}
