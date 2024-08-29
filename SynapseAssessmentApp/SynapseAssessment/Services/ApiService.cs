using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace SynapseAssessment.Services;

/// <summary>
/// Api service class that implements the IApiService interface.
/// This class performs the basic operations necessary to communicate with the provided APIs
/// and to process the orders received.
/// </summary>
public class ApiService : IApiService
{
	private readonly IConfiguration _config;
	private readonly ILogger<ApiService> _log;

	/// <summary>
	/// Constructor for ApiService
	/// </summary>
	/// <param name="config">IConfiguration object</param>
	/// <param name="log">ILogger object</param>
	public ApiService(IConfiguration config, ILogger<ApiService> log)
	{
		_config = config;
		_log = log;
	}

	/// <summary>
	/// Get the medical equipment orders for processing
	/// </summary>
	/// <returns>JArray of the order data returned from the Api or an empty array when unsuccessful response code returned</returns>
	public async Task<JObject[]> FetchMedicalEquipmentOrders()
	{
		using (HttpClient httpClient = new HttpClient())
		{
			string ordersApiUrl = _config.GetValue<string>("ApiUrls:OrdersApi");
			var response = await httpClient.GetAsync(ordersApiUrl);
			if (response.IsSuccessStatusCode)
			{
				var ordersData = await response.Content.ReadAsStringAsync();
				return JArray.Parse(ordersData).ToObject<JObject[]>();
			}
			else
			{
				_log.LogWarning("Failed to fetch orders from API.");
				return new JObject[0];
			}
		}
	}

	/// <summary>
	/// Process a delivered order by sending an alert and updating the order notification count
	/// </summary>
	/// <param name="order">JObject of the order data</param>
	/// <returns>Updated JObject of the order after processing</returns>
	public JObject ProcessOrder(JObject order)
	{
		var items = order["Items"].ToObject<JArray>();
		foreach (var item in items)
		{
			if (IsItemDelivered(item))
			{
				SendAlertMessage(item, order["OrderId"].ToString());
				IncrementDeliveryNotification(item);
			}
		}
		return order;
	}

	/// <summary>
	/// Updates the order with the current order information.
	/// </summary>
	/// <param name="order">JObject of order</param>
	public async Task UpdateOrder(JObject order)
	{
		using (HttpClient httpClient = new HttpClient())
		{
			string updateApiUrl = _config.GetValue<string>("ApiUrls:UpdateApi");
			var content = new StringContent(order.ToString(), System.Text.Encoding.UTF8, "application/json");
			var response = await httpClient.PostAsync(updateApiUrl, content);

			if (response.IsSuccessStatusCode)
			{
				_log.LogInformation("Updated order sent for processing: {OrderId}", order["OrderId"]);
			}
			else
			{
				_log.LogWarning("Failed to send updated order for processing: {OrderId}", order["OrderId"]);
			}
		}
	}

	/// <summary>
	/// Validate that the api endpoints are configured
	/// </summary>
	/// <returns>Boolean indicating if the api endpoints are configured</returns>
	public bool ValidateApiEndpoints()
	{
		string ordersApiUrl = _config.GetValue<string>("ApiUrls:OrdersApi");
		string alertApiUrl = _config.GetValue<string>("ApiUrls:AlertApi");
		string updateApiUrl = _config.GetValue<string>("ApiUrls:UpdateApi");

		if (string.IsNullOrEmpty(ordersApiUrl) || string.IsNullOrEmpty(alertApiUrl) || string.IsNullOrEmpty(updateApiUrl))
		{
			return false;
		}

		// Additional checks could be added here to test the urls using api health check endpoints if implemented

		return true;
	}

	/// <summary>
	/// Increments the delivery notification count
	/// </summary>
	/// <param name="item">JToken object with value to increment</param>
	private void IncrementDeliveryNotification(JToken item)
	{
		item["deliveryNotification"] = item["deliveryNotification"].Value<int>() + 1;
	}

	/// <summary>
	/// Check if the item is delivered
	/// </summary>
	/// <param name="item">JToken object containint the status to check</param>
	/// <returns>Boolean indicating delivered status</returns>
	private bool IsItemDelivered(JToken item)
	{
		return item["Status"].ToString().Equals("Delivered", StringComparison.OrdinalIgnoreCase);
	}

	/// <summary>
	/// Delivery alert
	/// </summary>
	/// <param name="orderId">The order id for the alert</param>
	private void SendAlertMessage(JToken item, string orderId)
	{
		using (HttpClient httpClient = new HttpClient())
		{
			string alertApiUrl = _config.GetValue<string>("ApiUrls:AlertApi");
			var alertData = new
			{
				Message = $"Alert for delivered item: Order {orderId}, Item: {item["Description"]}, " +
						  $"Delivery Notifications: {item["deliveryNotification"]}"
			};
			var content = new StringContent(JObject.FromObject(alertData).ToString(), System.Text.Encoding.UTF8, "application/json");
			var response = httpClient.PostAsync(alertApiUrl, content).Result;

			if (response.IsSuccessStatusCode)
			{
				_log.LogInformation("Alert sent for delivered item: {description}", item["Description"]);
			}
			else
			{
				_log.LogWarning("Failed to send alert for delivered item: {description}", item["Description"]);
			}
		}
	}
}
