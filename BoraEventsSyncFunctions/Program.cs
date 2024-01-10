using BoraCrawlers;
using BoraEventsSyncFunctions.BoraHttp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;

var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureServices((appBuilder, services) =>
	{
		services.AddHttpClient<BoraHttpClient>((provider, httpClient) =>
		{
			var boraApiJwt = appBuilder.Configuration.GetSection("BoraApiJwt").Value;
			var boraApiDomain = appBuilder.Configuration.GetSection("BoraApiDomain").Value;
			httpClient.BaseAddress = new Uri(boraApiDomain);
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", boraApiJwt);
		});

		services.AddHttpClient<EventbriteHttpClient>((provider, httpClient) =>
		{
			var eventbriteApiJwt = appBuilder.Configuration.GetSection("EventbriteApiToken").Value;
			var eventbriteApiDomain = appBuilder.Configuration.GetSection("EventbriteApiDomain").Value;
			httpClient.BaseAddress = new Uri(eventbriteApiDomain);
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", eventbriteApiJwt);
		});
	})
	.Build();

host.Run();
