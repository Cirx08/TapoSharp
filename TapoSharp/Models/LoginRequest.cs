namespace TapoSharp.Models
{
    using System.Text.Json.Serialization;
    using TapoSharp.Helpers;

    public class LoginRequest : TapoParamRequest<LoginRequestParams>
    {
        public LoginRequest()
            : this(string.Empty, string.Empty)
        {
        }

        public LoginRequest(string username, string password)
            : base("login_device")
        {
            this.Params = new LoginRequestParams()
            {
                Username = EncryptionHelper.Encode(username, true),
                Password = EncryptionHelper.Encode(password)
            };
        }
    }

    public class LoginRequestParams
    {
        public LoginRequestParams()
        {
            this.Username = string.Empty;
            this.Password = string.Empty;
        }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class LoginResponse : TapoResultResponse<LoginResponseResult>
    {
        public LoginResponse()
            : base()
        {
        }
    }

    public class LoginResponseResult
    {
        public LoginResponseResult()
        {
            this.Token = string.Empty;
        }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}