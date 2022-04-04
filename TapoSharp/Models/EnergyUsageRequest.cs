namespace TapoSharp.Models
{
    using System.Text.Json.Serialization;

    public class EnergyUsageRequest : TapoRequest
    {
        public EnergyUsageRequest()
            : base("get_energy_usage")
        {
        }
    }

    public class EnergyUsageResponse : TapoResultResponse<EnergyUsage>
    {
        public EnergyUsageResponse()
            : base()
        {
        }
    }

    public class EnergyUsage
    {
        public EnergyUsage()
        {
            this.TodayRuntime = 0;
            this.MonthRuntime = 0;
            this.TodayEnergy = 0;
            this.MonthEnergy = 0;
            this.LocalTimeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.Past24h = new long[0];
            this.Past7d = new long[0][];
            this.Past30d = new long[0];
            this.Past1y = new long[0];
            this.CurrentPower = 0;
        }

        [JsonPropertyName("today_runtime")]
        public long TodayRuntime { get; set; }

        [JsonPropertyName("month_runtime")]
        public long MonthRuntime { get; set; }

        [JsonPropertyName("today_energy")]
        public long TodayEnergy { get; set; }

        [JsonPropertyName("month_energy")]
        public long MonthEnergy { get; set; }

        [JsonPropertyName("local_time")]
        public string LocalTimeString { get; set; }

        [JsonIgnore]
        public DateTime? LocalTime 
        {
            get 
            {
                DateTime.TryParse(LocalTimeString, out var dateTime);
                return dateTime;
            }
        }

        [JsonPropertyName("past24h")]
        public long[] Past24h { get; set; }

        [JsonPropertyName("past7d")]
        public long[][] Past7d { get; set; }
        
        [JsonPropertyName("past30d")]
        public long[] Past30d { get; set; }

        [JsonPropertyName("past1y")]
        public long[] Past1y { get; set; }

        [JsonPropertyName("current_power")]
        public long CurrentPower { get; set; }
    }
}