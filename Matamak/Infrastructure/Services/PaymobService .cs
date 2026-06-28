
using System.Net.Http.Json;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class PaymobService : IPaymobService
    {
        private readonly PaymobSettings _settings;
        private readonly HttpClient _httpClient;

        // ✅ غيرت PaymobSettings لـ IOptions<PaymobSettings>
        public PaymobService(PaymobSettings settings, HttpClient httpClient)
        {
            _settings = settings;
            _httpClient = httpClient;
        }

        private async Task<string> GetAuthTokenAsync()
        {
            var response = await _httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/auth/tokens",
                new { api_key = _settings.ApiKey }
            );
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            return result.GetProperty("token").GetString();
        }

        private async Task<int> CreateOrderAsync(string authToken, int amountCents)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/ecommerce/orders",
                new
                {
                    auth_token = authToken,
                    delivery_needed = true,
                    amount_cents = amountCents,
                    currency = "EGP"
                }
            );
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            return result.GetProperty("id").GetInt32();
        }

        private async Task<string> GetPaymentKeyAsync(string authToken, int paymobOrderId, int amountCents, string customerEmail)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "https://accept.paymob.com/api/acceptance/payment_keys",
                new
                {
                    auth_token = authToken,
                    amount_cents = amountCents,
                    expiration = 3600,
                    order_id = paymobOrderId,
                    billing_data = new
                    {
                        email = customerEmail,
                        first_name = "Customer",
                        last_name = ".",
                        phone_number = "01000000000",
                        apartment = "NA",
                        floor = "NA",
                        street = "NA",
                        building = "NA",
                        shipping_method = "NA",
                        postal_code = "NA",
                        city = "NA",
                        country = "EG",
                        state = "NA"
                    },
                    currency = "EGP",
                    integration_id = _settings.IntegrationId
                }
            );
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            return result.GetProperty("token").GetString();
        }

        public async Task<string> GetPaymentUrlAsync(int orderId, int amountCents, string customerEmail)
        {
            var authToken = await GetAuthTokenAsync();
            var paymobOrderId = await CreateOrderAsync(authToken, amountCents);
            var paymentKey = await GetPaymentKeyAsync(authToken, paymobOrderId, amountCents, customerEmail);

            return $"https://accept.paymob.com/api/acceptance/iframes/{_settings.IframeId}?payment_token={paymentKey}";
        }
    }
}