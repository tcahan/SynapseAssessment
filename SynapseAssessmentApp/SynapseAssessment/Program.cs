using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = new ConfigurationBuilder();
BuildConfig(builder);
IConfiguration configuration = builder.Build();

// Test code to be removed later
Console.WriteLine($"Reading Test config: {configuration.GetValue<string>("Test")}");

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