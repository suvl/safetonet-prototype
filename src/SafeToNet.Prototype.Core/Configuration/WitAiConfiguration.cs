namespace SafeToNet.Prototype.Core.Configuration
{
    public class WitAiConfiguration
    {
        public string BaseUrl { get; set; }
        public string ApiVersion { get; set; }
        public string ApiKey { get; set; }

        public string MessageResource { get; set; } = "/message?v={version}&q={query}";
    }
}