namespace TapoSharp.Models
{
    using System.Net;

    public class TapoDeviceDiscoveredEventArgs
    {
        public TapoDeviceDiscoveredEventArgs()
            : this(IPAddress.None)
        {
        }

        public TapoDeviceDiscoveredEventArgs(IPAddress ip)
        {
            this.IpAddress = ip;
        }

        public IPAddress IpAddress { get; set; }
    }
}