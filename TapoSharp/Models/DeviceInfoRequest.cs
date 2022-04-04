namespace TapoSharp.Models
{
    using System.Text.Json.Serialization;
    using TapoSharp.Enums;
    using TapoSharp.Helpers;

    public class DeviceInfoRequest : TapoRequest
    {
        public DeviceInfoRequest()
            : base("get_device_info")
        {
        }
    }

    public class DeviceInfoRequestParams
    {
        public DeviceInfoRequestParams()
            : base()
        {
        }
    }

    public class DeviceInfoResponse : TapoResultResponse<DeviceInfo>
    {
        public DeviceInfoResponse()
            : base()
        {
        }
    }

    public class DeviceInfo
    {
        public DeviceInfo()
        {
            this.DeviceId = string.Empty;
            this.DeviceType = string.Empty;
            this.FirmwareId = string.Empty;
            this.FirmwareVersion = string.Empty;
            this.HardwareId = string.Empty;
            this.HardwareVersion = string.Empty;
            this.Model = string.Empty;
            this.MacAddress = string.Empty;
            this.OemId = string.Empty;
            this.Overheated = false;
            this.IpAddress = string.Empty;
            this.TimeDifference = 0;
            this.SSID_Encoded = string.Empty;
            this.RSSI = 0;
            this.SignalLevel = 0;
            this.Latitude = 0;
            this.Longitude = 0;
            this.Lang = string.Empty;
            this.Avatar = string.Empty;
            this.Region = string.Empty;
            this.Specs = string.Empty;
            this.Nickname_Encoded = string.Empty;
            this.HasSetLocationInfo = false;
            this.IsDeviceOn = false;
            this.Uptime = 0;
            this.DefaultStates = null;
        }

        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; }
        
        [JsonPropertyName("type")]
        public string DeviceType { get; set; }

        [JsonPropertyName("fw_id")]
        public string FirmwareId { get; set; }

        [JsonPropertyName("fw_ver")]
        public string FirmwareVersion { get; set; }

        [JsonPropertyName("hw_id")]
        public string HardwareId { get; set; }

        [JsonPropertyName("hw_ver")]
        public string HardwareVersion { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("mac")]
        public string MacAddress { get; set; }

        [JsonPropertyName("oem_id")]
        public string OemId { get; set; }

        [JsonPropertyName("overheated")]
        public bool Overheated { get; set; }

        [JsonPropertyName("ip")]
        public string IpAddress { get; set; }

        [JsonPropertyName("time_diff")]
        public long TimeDifference { get; set; }

        [JsonPropertyName("ssid")]
        public string SSID_Encoded { get; set; }

        [JsonIgnore]
        public string SSID 
        {
            get 
            {
                return !string.IsNullOrEmpty(this.SSID_Encoded) ? EncryptionHelper.Decode(this.SSID_Encoded) : string.Empty;
            }
        }

        [JsonPropertyName("rssi")]
        public int RSSI { get; set; }

        [JsonPropertyName("signal_level")]
        public int SignalLevel { get; set; }

        [JsonPropertyName("latitude")]
        public int Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public int Longitude { get; set; }

        [JsonPropertyName("lang")]
        public string Lang { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("specs")]
        public string Specs { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname_Encoded { get; set; }

        [JsonIgnore]
        public string Nickname
        {
            get
            {
                return !string.IsNullOrEmpty(this.Nickname_Encoded) ? EncryptionHelper.Decode(this.Nickname_Encoded) : string.Empty;
            }
        }

        [JsonPropertyName("has_set_location_info")]
        public bool HasSetLocationInfo { get; set; }

        [JsonPropertyName("device_on")]
        public bool IsDeviceOn { get; set; }

        [JsonIgnore]
        public PowerState State 
        {
            get 
            {
                return this.IsDeviceOn ? PowerState.ON : PowerState.OFF;
            }
        }

        [JsonPropertyName("on_time")]
        public long Uptime { get; set; }

        [JsonPropertyName("default_states")]
        public DeviceState DefaultStates { get; set; }
    }

    public class DeviceState
    {
        public DeviceState()
        {
            this.Type = string.Empty;
            //this.State = string.Empty;
        }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        //[JsonPropertyName("state")]
        //public string State { get; set; }
    }
}