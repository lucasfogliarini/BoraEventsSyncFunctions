namespace BoraEventsSyncFunctions.BoraHttp
{
	public class AuthenticationInput
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string PhotoUrl { get; set; } = "na";
		public string Provider { get; set; } = "BORA_SYNC";
	}
}
