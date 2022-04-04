namespace TapoSharp.Clients
{
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using TapoSharp.Helpers;
    using TapoSharp.Models;

    public class P110Client : P100Client
    {
        public P110Client(IPAddress ip)
            : base(ip)
        {
        }

        public P110Client(IPAddress ip, int defaultRequestTimeout)
            : base(ip, defaultRequestTimeout)
        {
        }

        public EnergyUsage GetEnergyUsage()
        {
            return this.GetEnergyUsageAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<EnergyUsage> GetEnergyUsageAsync()
        {
            if (_client != null)
            {
                try
                {
                    var securePayload = new TapoSecureRequest<EnergyUsageRequest>(new EnergyUsageRequest(), _tapoKey, _tapoIv);
                    var response = await _client.PostAsync($"/app?token={_token}", new StringContent(JsonSerializer.Serialize(securePayload)));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var secureResponse = JsonSerializer.Deserialize<TapoSecureResultResponse>(await response.Content.ReadAsStringAsync());
                        if (secureResponse?.Result?.Response != null)
                        {
                            var stateResponse = EncryptionHelper.Decrypt(secureResponse.Result.Response, _tapoKey, _tapoIv);
                            var resp = JsonSerializer.Deserialize<EnergyUsageResponse>(stateResponse)?.Result;
                            if (resp != null)
                            {
                                return resp;
                            }
                        }
                    }
                }
                catch { }
            }

            return null;
        }
    }
}