using System.Net;

namespace BoraEventsSyncFunctions.BoraHttp
{
	public class ErrorDetails
	{
		public string? type { get; set; }
		public string? title { get; set; }
		public HttpStatusCode status { get; set; }
		public string? detail { get; set; }
		public string? traceId { get; set; }
	}
}
