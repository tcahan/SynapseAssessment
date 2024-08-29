using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SynapseAssessment.Services;

namespace SynapseAssessment.MainApp;

public class MainApp : IMainApp
{
	private readonly IConfiguration _config;
	private readonly ILogger<MainApp> _log;
	private readonly IApiService _api;

	public MainApp(IConfiguration config, ILogger<MainApp> log, IApiService api)
	{
		_config = config;
		_log = log;
		_api = api;
	}

	public void Start()
	{
		_log.LogInformation("Start of App");

		try
		{
			if (_api.ValidateApiEndpoints() == false)
			{
				_log.LogError("API endpoints are not valid. Please check the configuration.");
				return;
			}

			var medicalEquipmentOrders = _api.FetchMedicalEquipmentOrders().GetAwaiter().GetResult();
			foreach (var order in medicalEquipmentOrders)
			{
				var updatedOrder = _api.ProcessOrder(order);
				_api.UpdateOrder(updatedOrder).GetAwaiter().GetResult();
			}

			_log.LogInformation("Results sent to relevant APIs.");
		}
		catch (Exception ex)
		{
			// Additionally we could provide the stack trace here, but that would be messy for a simple log message.
			// This could be handled in a separate table which includes specific details such as the class name and method as well as
			// including an error id that can be returned the user for reference and bug ticket creation.
			// Another option is to include logging of the stack trace at a different log level, such as Debug, that will only be shown
			// when investigating a known issue. This will have the downside of not being available for rare issues that are not known.
			_log.LogError("An error has occurred! Error provided: {msg}", ex.Message);
		}
	}
}
