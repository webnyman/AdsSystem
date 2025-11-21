using System.Net.Http.Json;

namespace Ads.Web.Services
{
    public class SubscriberAdInfoDto
    {
        public string SubscriptionNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool AllowedToAdvertise { get; set; }
    }
    public class SubscriberContactUpdateDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class SubscriberApiClient
    {
        private readonly HttpClient _http;

        public SubscriberApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<SubscriberAdInfoDto?> GetAdInfoAsync(string subscriptionNumber, CancellationToken ct = default)
        {
            // Endpointen vi gjorde igår: /api/Subscribers/{prenr}/ad-info
            var url = $"/api/Subscribers/{Uri.EscapeDataString(subscriptionNumber)}/ad-info";
            return await _http.GetFromJsonAsync<SubscriberAdInfoDto>(url, ct);
        }
        public async Task<bool> UpdateContactAsync(string subscriptionNumber, SubscriberContactUpdateDto dto, CancellationToken ct = default)
        {
            var url = $"/api/Subscribers/{Uri.EscapeDataString(subscriptionNumber)}/contact";
            var response = await _http.PutAsJsonAsync(url, dto, ct);
            return response.IsSuccessStatusCode;
        }

    }
}
