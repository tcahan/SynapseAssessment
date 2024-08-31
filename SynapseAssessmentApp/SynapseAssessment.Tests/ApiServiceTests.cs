using Xunit;
using Newtonsoft.Json.Linq;
using SynapseAssessment.Services;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Net;
using RichardSzalay.MockHttp;

namespace SynapseAssessment.Tests
{
	public class ApiServiceTests
	{
		private readonly Mock<IConfiguration> _configMock = new Mock<IConfiguration>();
		private readonly Mock<ILogger<ApiService>> _logMock = new Mock<ILogger<ApiService>>();
		private readonly MockHttpMessageHandler _handlerMock;
		private readonly string _sampleOrderJson;

		public ApiServiceTests()
		{
			_handlerMock = new MockHttpMessageHandler();
			_sampleOrderJson = "[{\"OrderId\": 1,\"Items\": [{\"Description\": \"My item\",\"deliveryNotification\": 0,\"Status\": \"Delivered\"}]}]";
		}

		[Theory]
		[InlineData("https://orders-api.com/orders", "https://alert-api.com/alerts", "https://update-api.com/update", true)]
		[InlineData("", "https://alert-api.com/alerts", "https://update-api.com/update", false)]
		[InlineData("https://orders-api.com/orders", "", "https://update-api.com/update", false)]
		[InlineData("https://orders-api.com/orders", "https://alert-api.com/alerts", "", false)]
		public void ValidateApiEndpoints_ReturnsCorrectly(string orderApi, string alertApi, string updateApi, bool expected)
		{
			// Given
			var ordersApiSection = new Mock<IConfigurationSection>();
			ordersApiSection.Setup(x => x.Value).Returns(orderApi);
			_configMock.Setup(x => x.GetSection("ApiUrls:OrdersApi")).Returns(ordersApiSection.Object);

			var alertApiSection = new Mock<IConfigurationSection>();
			alertApiSection.Setup(x => x.Value).Returns(alertApi);
			_configMock.Setup(x => x.GetSection("ApiUrls:AlertApi")).Returns(alertApiSection.Object);

			var updateApiSection = new Mock<IConfigurationSection>();
			updateApiSection.Setup(x => x.Value).Returns(updateApi);
			_configMock.Setup(x => x.GetSection("ApiUrls:UpdateApi")).Returns(updateApiSection.Object);

			var _api = new ApiService(_configMock.Object, _logMock.Object, null);

			// When
			var result = _api.ValidateApiEndpoints();

			// Then
			Assert.Equal(expected, result);
		}

		[Fact]
		public async Task FetchMedicalEquipmentOrders_HandlesNotSuccess()
		{
			// Given
			var expected = new JObject[0];
			var ordersApiSection = new Mock<IConfigurationSection>();
			ordersApiSection.Setup(x => x.Value).Returns("https://orders-api.com/orders");
			_configMock.Setup(x => x.GetSection("ApiUrls:OrdersApi"))
				.Returns(ordersApiSection.Object);

			_handlerMock.When("https://orders-api.com/orders").Respond(HttpStatusCode.Forbidden);

			var _api = new ApiService(_configMock.Object, _logMock.Object, _handlerMock.ToHttpClient());

			// When
			var result = await _api.FetchMedicalEquipmentOrders();

			// Then			
			Assert.Equal(expected, result);
		}

		[Fact]
		public async Task FetchMedicalEquipmentOrders_HandlesSuccess()
		{
			// Given
			var expected = JArray.Parse(_sampleOrderJson).ToObject<JObject[]>();
			var ordersApiSection = new Mock<IConfigurationSection>();
			ordersApiSection.Setup(x => x.Value).Returns("https://orders-api.com/orders");
			_configMock.Setup(x => x.GetSection("ApiUrls:OrdersApi"))
				.Returns(ordersApiSection.Object);

			_handlerMock.When("https://orders-api.com/orders").Respond(HttpStatusCode.OK, "application/json", _sampleOrderJson);

			var _api = new ApiService(_configMock.Object, _logMock.Object, _handlerMock.ToHttpClient());

			// When
			var result = await _api.FetchMedicalEquipmentOrders();

			// Then
			Assert.Equal(expected, result);
		}
	}
}
