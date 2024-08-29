using Newtonsoft.Json.Linq;

namespace SynapseAssessment.Services
{
	public interface IApiService
	{
		Task<JObject[]> FetchMedicalEquipmentOrders();
		JObject ProcessOrder(JObject order);
		Task UpdateOrder(JObject order);
		bool ValidateApiEndpoints();
	}
}