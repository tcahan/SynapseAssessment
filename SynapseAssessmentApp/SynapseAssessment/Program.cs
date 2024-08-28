using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

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
	.UseSerilog()
	.Build();

// Test code to be removed later
Log.Logger.Information("Reading Test config: {value}", configuration.GetValue<string>("Test"));
Log.Logger.Debug("This is a debug message; It should not be logged at the current Infomration level");


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