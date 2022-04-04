# TapoSharp

TapoSharp is a .NET implementation of the Tapo Smart Plug API. It is written in .NET 6

## Getting Started

### Scanning for devices

```
var scanRange = "192.168.1.0/24"; // CIDR notation. /32 for single IP, /24 for 254 IPs
var clients = TapoClient.ScanForDevices(scanRange);
Console.WriteLine($"Found '{clients.Count}' devices in the range: '{scanRange}'");
```

### Attaching an event handler

```
var _username = "{ENTER_TAPO_ACCOUNT_EMAIL_HERE}";
var _password = "{ENTER_TAPO_ACCOUNT_PASSWORD_HERE}";

TapoClient.OnDeviceDiscovered += (s, e) => 
{
	// Attempt to login
    if (e.Client.Login(_username, _password, e.Key))
    {
        var info = e.Client.GetDeviceInfo();
        Console.WriteLine($"Found '{info.Nickname} - {info.MacAddress} - {info.IpAddress}'");
    }
};

var scanRange = "192.168.1.0/24"; // CIDR notation. /32 for single IP, /24 for 254 IPs
var clients = TapoClient.ScanForDevices(scanRange);
```

### Getting device information

```
var _username = "{ENTER_TAPO_ACCOUNT_EMAIL_HERE}";
var _password = "{ENTER_TAPO_ACCOUNT_PASSWORD_HERE}";

var device = IPAddress.Parse("192.168.1.10");

Console.WriteLine($"Connecting to device: '{device.ToString()}'");
var client = new TapoClient(device);

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
    }
}
```

### Setting device state (On/Off)

```
var _username = "{ENTER_TAPO_ACCOUNT_EMAIL_HERE}";
var _password = "{ENTER_TAPO_ACCOUNT_PASSWORD_HERE}";

var device = IPAddress.Parse("192.168.1.10");

Console.WriteLine($"Connecting to device: '{device.ToString()}'");
var client = new P100Client(device);

Console.WriteLine($"Sending handshake to device: '{device.ToString()}'");
var handshakeResponse = client.Handshake();
if (handshakeResponse != null)
{
    Console.WriteLine($"Logging into device: '{device.ToString()}'");
    if (client.Login(_username, _password, handshakeResponse.Key))
    {
        Console.WriteLine($"Turning device ON: '{device.ToString()}'");
        var stateResponse = client.ChangeState(PowerState.ON);
        Console.WriteLine($"Turning device OFF: '{device.ToString()}'");
        stateResponse = client.ChangeState(PowerState.OFF);
    }
}
```

### Getting device power consumption (P110 Only)

```
var _username = "{ENTER_TAPO_ACCOUNT_EMAIL_HERE}";
var _password = "{ENTER_TAPO_ACCOUNT_PASSWORD_HERE}";

var device = IPAddress.Parse("192.168.1.10");

Console.WriteLine($"Connecting to device: '{device.ToString()}'");
var client = new P110Client(device);

Console.WriteLine($"Sending handshake to device: '{device.ToString()}'");
var handshakeResponse = client.Handshake();
if (handshakeResponse != null)
{
    Console.WriteLine($"Logging into device: '{device.ToString()}'");
    if (client.Login(_username, _password, handshakeResponse.Key))
    {
        Console.WriteLine($"Getting energy usage for: '{device.ToString()}'");
        var usage = client.GetEnergyUsage();
    }
}
```

## Authors

* [@Cirx08](https://github.com/Cirx08)

## Version History

* 0.1
    * Initial Release

## License

This project is licensed under the MIT License

## Acknowledgments

Thanks to the guys over on the Home Assistant community forum for their great work reverse engineering the device. Without them this API would not exist.
* [Home Assistant Community](https://community.home-assistant.io/t/tp-link-tapo-p100/147792)