using System.Net;
using TapoSharp.Clients;
using TapoSharp.Enums;

var _username = "{ENTER_TAPO_ACCOUNT_EMAIL_HERE}";
var _password = "{ENTER_TAPO_ACCOUNT_PASSWORD_HERE}";

var scanRange = "192.168.1.0/24";
Console.WriteLine($"Scanning range for devices: '{scanRange}'");
TapoClient.ScanForDevices(scanRange, FilterEnum.Usable, (ip, tapoClient) =>
{
    if (tapoClient.Login(_username, _password))
    {
        var info = tapoClient.GetDeviceInfo();
        Console.WriteLine(info.Print());

        if (info.ModelType == DeviceType.P110)
        {
            var client = new P110Client();
            if (client.Login(_username, _password))
            { 
                var usage = client.GetEnergyUsage();
                Console.WriteLine($"Current power consumption: '{usage.CurrentPower_W.ToString("0.00")}W'");
            }
        }
    }
});

Thread.Sleep(Timeout.Infinite);