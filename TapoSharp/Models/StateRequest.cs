namespace TapoSharp.Models
{
    using System.Text.Json.Serialization;

    public class StateRequest : TapoParamRequest<StateRequestParams>
    {
        public StateRequest()
            : this(false)
        {
        }

        public StateRequest(bool on)
            : base("set_device_info")
        {
            this.Params = new StateRequestParams()
            {
                DeviceOn = on
            };
        }
    }

    public class StateRequestParams : TapoRequestParam
    {
        public StateRequestParams()
            : base()
        {
            this.DeviceOn = false;
        }

        [JsonPropertyName("device_on")]
        public bool DeviceOn { get; set; }
    }

    public class StateResponse : TapoResponse
    {
        public StateResponse()
            : base()
        {
        }
    }
}