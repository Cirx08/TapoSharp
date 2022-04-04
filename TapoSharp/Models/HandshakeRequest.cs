namespace TapoSharp.Models
{
    using System.Text.Json.Serialization;

    public class HandshakeRequest : TapoParamRequest<HandshakeRequestParams>
    {
        public HandshakeRequest(string uuid)
            : this(uuid, string.Empty)
        {
            this.TerminalUUID = uuid;
        }

        public HandshakeRequest(string uuid, string publicKey)
            : base("handshake")
        {
            this.Params = new HandshakeRequestParams()
            {
                Key = publicKey
            };
            this.TerminalUUID = uuid;
        }

        [JsonPropertyName("terminalUUID")]
        public string TerminalUUID { get; set; }
    }

    public class HandshakeRequestParams : TapoRequestParam
    {
        public HandshakeRequestParams()
            : base()
        {
            this.Key = string.Empty;
        }

        [JsonPropertyName("key")]
        public string Key { get; set; }
    }

    public class HandshakeResponse : TapoResultResponse<Handshake>
    {
        public HandshakeResponse()
            : base()
        {
        }
    }

    public class Handshake
    {
        public Handshake()
        {
            this.Key = string.Empty;
        }

        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
}