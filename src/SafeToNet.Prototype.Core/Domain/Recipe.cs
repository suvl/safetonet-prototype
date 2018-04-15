using System;
using System.Collections.Generic;
using System.Text;

namespace SafeToNet.Prototype.Core.Domain
{
    public class Recipe
    {
        public string ImageUrl { get; set; }

        public string SourceUrl { get; set; }

        public string AggregatorUrl { get; set; }

        public string Title { get; set; }

        public string Publisher { get; set; }

        public string PublisherUrl { get; set; }

        public int SocialRank { get; set; }
    }
}
