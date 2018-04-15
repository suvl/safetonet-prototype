using System;
using System.Collections.Generic;
using System.Text;

namespace SafeToNet.Prototype.Core.Configuration
{
    public class Food2ForkConfiguration
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }

        public string SearchResource { get; set; } = "/api/search?key={key}&q={query}";
        public string GetResource { get; set; } = "/api/get?key={key}&rId={id}";
    }
}
