using AngleSharp.Dom;
using AngleSharp;
using System.Globalization;

namespace BoraCrawlers
{
    public class MinhaEntradaCrawler : BoraCrawler
    {
        const string MINHAENTRADA_DOMAIN = "https://minhaentrada.com.br";

		/// <summary>
		/// A crawler class for extracting event information from a 'https://minhaentrada.com.br'
		/// </summary>
		/// <param name="eventsQuery">https://minhaentrada.com.br/agenda-geral{eventsQuery}</param>
		public MinhaEntradaCrawler(string eventsQuery)
        {
            EventsSchedule = $"{MINHAENTRADA_DOMAIN}/agenda-geral{eventsQuery}";
        }
        protected override List<CrawledEvent> ExtractEvents(IDocument document)
        {
            var events = document.QuerySelectorAll(".card-agenda-geral");

            List<CrawledEvent> eventList = new();

            foreach (var eventNode in events)
            {
                var title = eventNode.QuerySelector("h4").TextContent.Trim();
                var dateTimeString = eventNode.QuerySelector(".zmdi-calendar-check").Parent.TextContent.Trim().Substring(6).Trim();
                var location = eventNode.QuerySelector(".zmdi-pin").Parent.TextContent.Trim();
                var imageUrl = eventNode.QuerySelector("img")?.GetAttribute("src");
                var eventLink = eventNode.QuerySelector("a.color-font-black")?.GetAttribute("href");
                eventLink = $"{MINHAENTRADA_DOMAIN}{eventLink}";
                var dateTime = ParseDateTime(dateTimeString, "dd/MM/yy - HH:mm'h'");
                dateTime ??= ParseDateTime(dateTimeString, "dd/MM/yyyy - HH:mm'h'");

				var eventData = new CrawledEvent
                {
                    Title = title,
                    DateTime = dateTime,
                    Location = location,
                    ImageUrl = imageUrl,
                    EventLink = eventLink
                };

                eventList.Add(eventData);
            }

            return eventList;
        }

        private static DateTime? ParseDateTime(string dateTimeString, string dateFormat)
        {
			DateTime.TryParseExact(dateTimeString, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime dateTime);
            return dateTime;
		}
    }
}
