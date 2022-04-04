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

var scanRange = "192.168.1.0/24"; // CIDR notation. /32 for single IP, /24 for 254 IPs
var clients = TapoClient.ScanForDevices(scanRange, FilterEnum.Usable, (ip, tapoClient) => 
{
    // Attempt to login
    var client = new P100Client(e.IpAddress);
    if (client.Login(_username, _password))
    {
        var info = client.GetDeviceInfo();
        Console.WriteLine(info.Print());
    }    
});
```

### Getting device information

```
var _username = "{ENTER_TAPO_ACCOUNT_EMAIL_HERE}";
var _password = "{ENTER_TAPO_ACCOUNT_PASSWORD_HERE}";

var device = IPAddress.Parse("192.168.1.10");

Console.WriteLine($"Connecting to device: '{device.ToString()}'");
var client = new TapoClient(device);

Console.WriteLine($"Logging into device: '{device.ToString()}'");
if (client.Login(_username, _password))
{
    var info = client.GetDeviceInfo();
    Console.WriteLine(info.Print());
}
```

### Setting device state (On/Off)

```
var _username = "{ENTER_TAPO_ACCOUNT_EMAIL_HERE}";
var _password = "{ENTER_TAPO_ACCOUNT_PASSWORD_HERE}";

var device = IPAddress.Parse("192.168.1.10");

Console.WriteLine($"Connecting to device: '{device.ToString()}'");
var client = new P100Client(device);

Console.WriteLine($"Logging into device: '{device.ToString()}'");
if (client.Login(_username, _password))
{
    Console.WriteLine($"Turning device ON: '{device.ToString()}'");
    client.ChangeState(PowerState.ON);

    Console.WriteLine($"Turning device OFF: '{device.ToString()}'");
    stateResponse = client.ChangeState(PowerState.OFF);
}
```

### Getting device power consumption (P110 Only)

```
var _username = "{ENTER_TAPO_ACCOUNT_EMAIL_HERE}";
var _password = "{ENTER_TAPO_ACCOUNT_PASSWORD_HERE}";

var device = IPAddress.Parse("192.168.1.10");

Console.WriteLine($"Connecting to device: '{device.ToString()}'");
var client = new P110Client(device);

Console.WriteLine($"Logging into device: '{device.ToString()}'");
if (client.Login(_username, _password))
{
    Console.WriteLine($"Getting energy usage for: '{device.ToString()}'");
    var usage = client.GetEnergyUsage();
    Console.WriteLine($"Current power consumption: '{usage.CurrentPower_W.ToString("0.00")}W'");
}
```

## Authors

* [@Cirx08](https://github.com/Cirx08)

## Version History

* 1.0.4
    * Added login state tracker
* 1.0.3
    * Fixed issue with multiple handshake requests
    * Fixed issue with scanning of /32 network
* 1.0.2
    * Added callback functions on scan discovery
* 1.0.1
    * Added a merged login and handshake request
    * Reworked the raised event args for OnDeviceDiscovered
    * Added default constructors to the clients
* 1.0.0
    * Initial Release

## License

This project is licensed under the GNU GPLv3 License - see the LICENSE.md file for details

## Acknowledgments

Thanks to the guys over on the Home Assistant community forum for their great work reverse engineering the device. Without them this API would not exist.
* [Home Assistant Community](https://community.home-assistant.io/t/tp-link-tapo-p100/147792)