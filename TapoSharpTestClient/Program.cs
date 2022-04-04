using TapoSharp.Clients;
using TapoSharp.Enums;

var _username = "{ENTER_TAPO_ACCOUNT_EMAIL_HERE}";
var _password = "{ENTER_TAPO_ACCOUNT_PASSWORD_HERE}";

var scanRange = "192.168.1.0/24";
Console.WriteLine($"Scanning range for devices: '{scanRange}'");
TapoClient.OnDeviceDiscovered += (s, e) => 
{
    var client = new P110Client(e.IpAddress);
    if (client.Login(_username, _password))
    { 
        var info = client.GetDeviceInfo();
        Console.WriteLine(info.Print());

        var usage = client.GetEnergyUsage();
        Console.WriteLine($"Current power consumption: '{usage.CurrentPower_W.ToString("0.00")}W'");

        if (info.State == PowerState.ON)
        {
            Console.WriteLine($"Turning device 'OFF'");
            client.ChangeState(PowerState.OFF);
            Console.WriteLine($"Turning device 'ON'");
            client.ChangeState(PowerState.ON);
        }
        else 
        {
            Console.WriteLine($"Turning device 'ON'");
            client.ChangeState(PowerState.ON);
            Console.WriteLine($"Turning device 'OFF'");
            client.ChangeState(PowerState.OFF);
        }
    }
};
TapoClient.ScanForDevices(scanRange);
Thread.Sleep(Timeout.Infinite);