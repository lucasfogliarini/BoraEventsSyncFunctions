using BoraEventsSyncFunctions.GoogleCalendar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureServices((appBuilder, services) =>
	{
		services.AddTransient<GoogleCalendarService>();
		services.AddTransient<AccountDataStore>();
		services.AddOptions<GoogleCalendarConfig>()
		   .Configure<IConfiguration>((settings, configuration) =>
		   {
			   configuration.GetSection(nameof(GoogleCalendarConfig)).Bind(settings);
		   });

	})
	.Build();

host.Run();
