namespace TapoSharp.Models
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using TapoSharp.Enums;
    using TapoSharp.Helpers;

    public class TapoRequest
    {
        public TapoRequest(string method)
        {
            this.Method = method;
        }

        [JsonPropertyName("method")]
        public string Method { get; private set; }
    }

    public class TapoParamRequest<T> : TapoRequest
    {
        public TapoParamRequest(string method)
            : base(method)
        {
            this.Params = Activator.CreateInstance<T>();
        }

        [JsonPropertyName("params")]
        public T Params { get; set; }
    }

    public class TapoSecureRequest<T> : TapoParamRequest<TapoSecureRequestParams>
    {
        public TapoSecureRequest()
            : this(string.Empty, new byte[0], new byte[0])
        {
        }

        public TapoSecureRequest(T payload, byte[] key, byte[] iv)
            : this(JsonSerializer.Serialize(payload), key, iv)
        {
        }

        public TapoSecureRequest(string payload, byte[] key, byte[] iv)
            : base("securePassthrough")
        {
            this.Params = new TapoSecureRequestParams()
            {
                Request = EncryptionHelper.Encrypt(payload, key, iv)
            };
        }
    }

    public class TapoSecureRequestParams
    {
        public TapoSecureRequestParams()
        {
            this.Request = string.Empty;
        }

        [JsonPropertyName("request")]
        public string Request { get; set; }
    }

    public class TapoResponse
    {
        public TapoResponse()
        {
            this.ErrorCode = (int)TapoErrorCode.UNKNOWN;
        }

        [JsonPropertyName("error_code")]
        public int ErrorCode { get; set; }

        [JsonIgnore]
        public TapoErrorCode Error
        {
            get
            {
                foreach (TapoErrorCode code in Enum.GetValues(typeof(TapoErrorCode)))
                {
                    if ((int)code == this.ErrorCode)
                    {
                        return code;
                    }
                }

                return TapoErrorCode.UNKNOWN;
            }
        }

        [JsonIgnore]
        public bool IsSuccess
        {
            get
            {
                return this.Error == TapoErrorCode.SUCCESS;
            }
        }
    }

    public class TapoResultResponse<T> : TapoResponse
    {
        public TapoResultResponse()
        {
            this.Result = Activator.CreateInstance<T>();
        }

        [JsonPropertyName("result")]
        public T Result { get; set; }
    }

    public class TapoRequestParam
    {
        public TapoRequestParam()
        {
            this.Key = string.Empty;
        }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("requestTimeMils")]
        public long RequestTimeMils 
        {
            get
            {
                return DateTimeOffset.Now.ToUnixTimeSeconds();
            }
        }
    }

    public class TapoSecureResultResponse : TapoResultResponse<TapoSecureResponseResult>
    {
        public TapoSecureResultResponse()
            : base()
        {
        }
    }

    public class TapoSecureResponseResult
    {
        public TapoSecureResponseResult()
        {
            this.Response = string.Empty;
        }

        [JsonPropertyName("response")]
        public string Response { get; set; }
    }
}