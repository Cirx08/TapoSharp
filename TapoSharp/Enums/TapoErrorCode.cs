namespace TapoSharp.Enums
{
    public enum TapoErrorCode
    {
        SUCCESS = 0,
        INVALID_PUBLIC_KEY_LENGTH = -1010,
        INVALID_TERMINAL_UUID = -1012,
        INVALID_REQUEST_OR_CREDENTIALS = -1501,
        INCORRECT_REQUEST = 1002,
        JSON_FORMATTING_ERROR = -1003,
        UNKNOWN = -9999
    }
}