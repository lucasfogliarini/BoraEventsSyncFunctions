using BoraEventsSyncFunctions.BoraHttp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureServices((appBuilder, services) =>
	{
		services.AddHttpClient<BoraHttpClient>();

	})
	.Build();

host.Run();
