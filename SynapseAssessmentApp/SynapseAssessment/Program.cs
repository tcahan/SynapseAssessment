using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SynapseAssessment.MainApp;
using SynapseAssessment.Services;


var builder = new ConfigurationBuilder();
BuildConfig(builder);
IConfiguration configuration = builder.Build();

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(configuration)
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.WriteTo.Debug()
	.CreateLogger();

var host = Host.CreateDefaultBuilder()
	.ConfigureServices((context, services) =>
	{
		services.AddTransient<IMainApp, MainApp>();
		services.AddScoped<IApiService, ApiService>();
		services.AddHttpClient();
	})
	.UseSerilog()
	.Build();

var mainApp = ActivatorUtilities.CreateInstance<MainApp>(host.Services);
mainApp.Start();

/// <summary>
/// Configures the configuration builder to support
/// reading configuration from appsettings.json, appsettings.{Environment}.json
/// </summary>
/// <param name="builder">The builder object to be configured by reference</param>
static void BuildConfig(IConfigurationBuilder builder)
{
	builder.SetBasePath(Directory.GetCurrentDirectory())
		.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
		.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
		.AddEnvironmentVariables();
}