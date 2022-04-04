namespace TapoSharp.Clients
{
    using System;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using TapoSharp.Helpers;
    using TapoSharp.Models;

    public class TapoClient
    {
        private const int DefaultRequestTimeout = 5; //Seconds
        public static event EventHandler<TapoDeviceDiscoveredEventArgs> OnDeviceDiscovered;

        private IPAddress _ip;
        private KeyPair _keyPair;
        private Guid _uuid;

        protected HttpClient _client;
        protected string _token;

        protected byte[] _tapoKey;
        protected byte[] _tapoIv;

        public TapoClient(IPAddress ip)
            : this(ip, DefaultRequestTimeout)
        {
        }

        public TapoClient(IPAddress ip, int defaultRequestTimeout)
        {
            _ip = ip;
            _keyPair = EncryptionHelper.GenerateKeyPair();
            _uuid = Guid.NewGuid();
            _token = string.Empty;
            _tapoKey = new byte[0];
            _tapoIv = new byte[0];
            
            _client = new HttpClient();
            _client.BaseAddress = new Uri($"http://{_ip.ToString()}");
            _client.Timeout = TimeSpan.FromSeconds(defaultRequestTimeout);
        }

        public static List<TapoClient> ScanForDevices(string network)
        {
            return ScanForDevicesAsync(network).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static List<TapoClient> ScanForDevices(IPNetwork network)
        {
            return ScanForDevicesAsync(network).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task<List<TapoClient>> ScanForDevicesAsync(string network)
        {
            return await ScanForDevicesAsync(IPNetwork.Parse(network));
        }

        public static async Task<List<TapoClient>> ScanForDevicesAsync(IPNetwork network, FilterEnum filter = FilterEnum.Usable)
        {
            var clients = new List<TapoClient>();

            var addressList = network.ListIPAddress(filter);
            if (addressList != null)
            {
                await Task.Run(() =>
                {
                    Parallel.ForEach(addressList, (address) =>
                    {
                        try
                        {
                            var client = new P100Client(address);
                            var token = client.Handshake();
                            if (!string.IsNullOrEmpty(token?.Key))
                            {
                                clients.Add(client);
                                client.DeviceDiscovered(new TapoDeviceDiscoveredEventArgs(client, token.Key));
                            }
                        }
                        catch { }
                    });
                });
            }

            return clients;
        }

        public Handshake Handshake()
        {
            return this.HandshakeAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<Handshake> HandshakeAsync()
        {
            if (_client != null)
            {
                try
                {
                    var response = await _client.PostAsync("/app", new StringContent(JsonSerializer.Serialize(new HandshakeRequest(_uuid.ToString(), _keyPair.PublicKey.Replace("\r", string.Empty)))));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(data))
                        {
                            var resp = JsonSerializer.Deserialize<HandshakeResponse>(data)?.Result;
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

        public bool Login(string username, string password, string key)
        {
            return this.LoginAsync(username, password, key).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<bool> LoginAsync(string username, string password, string key)
        {
            if (_client != null)
            {
                try
                {
                    var data = EncryptionHelper.RsaDecryption(key, _keyPair.PrivateKey);

                    _tapoKey = data.Take(16).ToArray();
                    _tapoIv = data.Skip(16).Take(16).ToArray();

                    var securePayload = new TapoSecureRequest<LoginRequest>(new LoginRequest(username, password), _tapoKey, _tapoIv);
                    var response = await _client.PostAsync("/app", new StringContent(JsonSerializer.Serialize(securePayload)));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var loginResponse = JsonSerializer.Deserialize<TapoSecureResultResponse>(await response.Content.ReadAsStringAsync());
                        if (loginResponse?.Result?.Response != null)
                        {
                            var tokenResponse = EncryptionHelper.Decrypt(loginResponse.Result.Response, _tapoKey, _tapoIv);
                            var token = JsonSerializer.Deserialize<LoginResponse>(tokenResponse);
                            if (!string.IsNullOrEmpty(token?.Result?.Token))
                            {
                                _token = token.Result.Token;
                                return true;
                            }
                        }
                    }
                }
                catch { }
            }

            return false;
        }

        public DeviceInfo GetDeviceInfo()
        {
            return this.GetDeviceInfoAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<DeviceInfo> GetDeviceInfoAsync()
        {
            if (_client != null)
            {
                try
                {
                    var securePayload = new TapoSecureRequest<DeviceInfoRequest>(new DeviceInfoRequest(), _tapoKey, _tapoIv);
                    var response = await _client.PostAsync($"/app?token={_token}", new StringContent(JsonSerializer.Serialize(securePayload)));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var secureResponse = JsonSerializer.Deserialize<TapoSecureResultResponse>(await response.Content.ReadAsStringAsync());
                        if (secureResponse?.Result?.Response != null)
                        {
                            var deviceInfoResponse = EncryptionHelper.Decrypt(secureResponse.Result.Response, _tapoKey, _tapoIv);
                            var resp = JsonSerializer.Deserialize<DeviceInfoResponse>(deviceInfoResponse)?.Result;
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

        protected virtual void DeviceDiscovered(TapoDeviceDiscoveredEventArgs e)
        {
            EventHandler<TapoDeviceDiscoveredEventArgs> handler = OnDeviceDiscovered;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}