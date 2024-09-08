using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Net;
using hardware_store_api.Models.Paypal;
using hardware_store_api.Models;
using hardware_store_api.Exceptions;

namespace hardware_store_api.Services.PaypalService
{
    public class PaypalService : IPaypalService
    {
        public async Task<PaypalToken> Authentication(string client_id, string secret_key)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var bytes = Encoding.UTF8.GetBytes($"{client_id}:{secret_key}");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));

                var keyValues = new List<KeyValuePair<string, string>>();
                keyValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));

                try
                {
                    HttpResponseMessage response = await httpClient.PostAsync("https://api-m.sandbox.paypal.com/v1/oauth2/token",
                        new FormUrlEncodedContent(keyValues));

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var token = JsonSerializer.Deserialize<PaypalToken>(content);
                        return token;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    
        public async Task<PaypalOrderDetails> GetOrder(string access_token, string order_id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
                
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync($"https://api-m.paypal.com/v2/checkout/orders/{order_id}");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var order = JsonSerializer.Deserialize<PaypalOrderDetails>(content);

                        if (order != null)
                        {
                            return order;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public async Task<PaypalOrder> CreateOrder(string access_token, List<PaypalProductModel> productsList)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

                    var postData = new
                    {
                        intent = "CAPTURE",
                        purchase_units = productsList
                    };

                    string jsonContent = JsonSerializer.Serialize(postData);
                    var body = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    try
                    {
                        HttpResponseMessage response = await httpClient.PostAsync("https://api-m.sandbox.paypal.com/v2/checkout/orders", body);

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var order = JsonSerializer.Deserialize<PaypalOrder>(content);

                            if (order != null)
                            {
                                return order;
                            }
                            else
                            {
                                throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error parsing paypal order data.");
                            }
                        }
                        else
                        {
                            throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error API paypal order creation.");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new HttpStatusException(HttpStatusCode.InternalServerError, "Error API paypal creation order request.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
