using System.Net;
using TapoSharp.Clients;
using TapoSharp.Enums;

var _username = "ENTER_EMAIL_HERE";
var _password = "ENTER_PASSWORD_HERE";

//var scanRange = "10.0.3.0/24";
//Console.WriteLine($"Scanning range for devices: '{scanRange}'");
//TapoClient.OnDeviceDiscovered += (s, e) => 
//{
//    if (e.Client.Login(_username, _password, e.Key))
//    {
//        var info = e.Client.GetDeviceInfo();
//        Console.WriteLine($"Found '{info.Nickname} - {info.MacAddress} - {info.IpAddress}'");
//    }
//};

//var clients = TapoClient.ScanForDevices(scanRange);
//Console.WriteLine($"Found '{clients.Count}' devices in the range: '{scanRange}'");

var device = IPAddress.Parse("10.0.3.26");

Console.WriteLine($"Connecting to device: '{device.ToString()}'");
var client = new P110Client(device);

Console.WriteLine($"Sending handshake to device: '{device.ToString()}'");
var handshakeResponse = client.Handshake();
if (handshakeResponse != null)
{
    Console.WriteLine($"Logging into device: '{device.ToString()}'");
    if (client.Login(_username, _password, handshakeResponse.Key))
    {
        Console.WriteLine($"Getting device info for: '{device.ToString()}'");
        var info = client.GetDeviceInfo();
        Console.WriteLine($"Device name is: '{info.Nickname}'");
        Console.WriteLine($"Device model is: '{info.Model}'");
        Console.WriteLine($"Device version is: '{info.FirmwareVersion}'");
        Console.WriteLine($"Device IP address is: '{info.IpAddress}'");
        Console.WriteLine($"Device MAC address is: '{info.MacAddress.Replace("-", ":")}'");

        Console.WriteLine($"Getting energy usage for: '{device.ToString()}'");
        var usage = client.GetEnergyUsage();

        Console.WriteLine($"Turning device ON: '{device.ToString()}'");
        var stateResponse = client.ChangeState(PowerState.ON);
        Console.WriteLine($"Turning device OFF: '{device.ToString()}'");
        stateResponse = client.ChangeState(PowerState.OFF);
    }
}