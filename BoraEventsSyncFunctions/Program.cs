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

	})
	.Build();

host.Run();
