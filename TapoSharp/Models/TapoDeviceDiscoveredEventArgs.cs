namespace TapoSharp.Models
{
    using TapoSharp.Clients;

    public class TapoDeviceDiscoveredEventArgs
    {
        public TapoDeviceDiscoveredEventArgs()
            : this(null, string.Empty)
        {
        }

        public TapoDeviceDiscoveredEventArgs(P100Client client, string key)
        {
            this.Client = client;
            this.Key = key;
        }

        public P100Client Client { get; set; }

        public string Key { get; set; }
    }
}