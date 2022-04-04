namespace TapoSharp.Clients
{
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using TapoSharp.Enums;
    using TapoSharp.Helpers;
    using TapoSharp.Models;

    public class P100Client : TapoClient
    {
        public P100Client()
            : base()
        {
        }

        public P100Client(IPAddress ip)
            : base(ip)
        {
        }

        public P100Client(IPAddress ip, int defaultRequestTimeout)
            : base(ip, defaultRequestTimeout)
        {
        }

        public bool ChangeState(PowerState state)
        {
            return this.ChangeStateAsync(state).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<bool> ChangeStateAsync(PowerState state)
        {
            if (_client != null)
            {
                try
                {
                    var securePayload = new TapoSecureRequest<StateRequest>(new StateRequest(state == PowerState.ON), _tapoKey, _tapoIv);
                    var response = await _client.PostAsync($"/app?token={_token}", new StringContent(JsonSerializer.Serialize(securePayload)));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var secureResponse = JsonSerializer.Deserialize<TapoSecureResultResponse>(await response.Content.ReadAsStringAsync());
                        if (secureResponse?.Result?.Response != null)
                        {
                            var stateResponse = EncryptionHelper.Decrypt(secureResponse.Result.Response, _tapoKey, _tapoIv);
                            var resp = JsonSerializer.Deserialize<TapoResponse>(stateResponse)?.IsSuccess;
                            if (resp != null)
                            {
                                return (bool)resp;
                            }
                        }
                    }
                }
                catch { }
            }

            return false;
        }
    }
}