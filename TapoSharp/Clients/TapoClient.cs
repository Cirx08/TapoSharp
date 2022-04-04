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
        public static event EventHandler<TapoDeviceDiscoveredEventArgs> OnDeviceDiscovered;

        private KeyPair _keyPair;
        private Guid _uuid;

        protected HttpClient _client;
        protected string _token;

        protected byte[] _tapoKey;
        protected byte[] _tapoIv;

        public TapoClient()
            : this(IPAddress.None)
        {
        }

        public TapoClient(IPAddress ip)
            : this(ip, 5)
        {
        }

        public TapoClient(IPAddress ip, int defaultRequestTimeout)
        {
            this.IpAddress = ip;
            this.IsLoggedIn = false;
            this.DefaultRequestTimeout = defaultRequestTimeout;
            this.InitClient();
        }

        public IPAddress IpAddress { get; private set; }
        
        public bool IsLoggedIn { get; private set; }

        private int DefaultRequestTimeout { get; set; }

        private void InitClient()
        {
            _keyPair = EncryptionHelper.GenerateKeyPair();
            _uuid = Guid.NewGuid();
            _token = string.Empty;
            _tapoKey = new byte[0];
            _tapoIv = new byte[0];

            _client = new HttpClient();
            _client.BaseAddress = new Uri($"http://{this.IpAddress.ToString()}");
            _client.Timeout = TimeSpan.FromSeconds(this.DefaultRequestTimeout);
        }

        public static List<TapoClient> ScanForDevices(string network, FilterEnum filter = FilterEnum.Usable, Action<IPAddress, TapoClient> action = null)
        {
            return ScanForDevicesAsync(network, filter, action).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static List<TapoClient> ScanForDevices(IPNetwork network, FilterEnum filter = FilterEnum.Usable, Action<IPAddress, TapoClient> action = null)
        {
            return ScanForDevicesAsync(network, filter, action).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task<List<TapoClient>> ScanForDevicesAsync(string network, FilterEnum filter = FilterEnum.Usable, Action<IPAddress, TapoClient> action = null)
        {
            return await ScanForDevicesAsync(IPNetwork.Parse(network), filter, action);
        }

        public static async Task<List<TapoClient>> ScanForDevicesAsync(IPNetwork network, FilterEnum filter = FilterEnum.Usable, Action<IPAddress, TapoClient> action = null)
        {
            var clients = new List<TapoClient>();

            var addressList = network.Cidr != 32 ? network.ListIPAddress(filter).ToList() : new List<IPAddress>() { network.FirstUsable };
            if (addressList != null)
            {
                await Task.Run(() =>
                {
                    Parallel.ForEach(addressList, (address) =>
                    {
                        try
                        {
                            var client = new TapoClient(address);
                            var token = client.Handshake();
                            if (!string.IsNullOrEmpty(token?.Key))
                            {
                                clients.Add(client);
                                client.DeviceDiscovered(new TapoDeviceDiscoveredEventArgs(address));

                                if (action != null)
                                {
                                    try
                                    {
                                        action(address, client);
                                    }
                                    catch { }
                                }
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
                    this.InitClient();
                    
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

        public bool Login(string username, string password)
        {
            return this.LoginAsync(username, password).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            if (!this.IsLoggedIn)
            { 
                var token = await this.HandshakeAsync();
                if (!string.IsNullOrEmpty(token?.Key))
                { 
                    await this.LoginAsync(username, password, token?.Key);
                }
            }

            return this.IsLoggedIn;
        }

        public bool Login(string username, string password, string key)
        {
            return this.LoginAsync(username, password, key).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task<bool> LoginAsync(string username, string password, string key)
        {
            if (_client != null && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(key) && !this.IsLoggedIn)
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
                                this.IsLoggedIn = true;
                            }
                        }
                    }
                }
                catch { }
            }

            return this.IsLoggedIn;
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