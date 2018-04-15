using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SafeToNet.Prototype.ExternalClients.Food2Fork
{
    /// <summary>
    /// Class Recipe.
    /// </summary>
    public class Recipe
    {
        /*
            "publisher": "The Pioneer Woman",
            "f2f_url": "http://food2fork.com/view/46996",
            "title": "Cajun Chicken Pasta",
            "source_url": "http://thepioneerwoman.com/cooking/2011/09/cajun-chicken-pasta/",
            "recipe_id": "46996",
            "image_url": "http://static.food2fork.com/cajun0676.jpg",
            "social_rank": 100,
            "publisher_url": "http://thepioneerwoman.com"
        */

        [JsonProperty("recipe_id")]
        public string RecipeId { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("source_url")]
        public string SourceUrl { get; set; }

        [JsonProperty("f2f_url")]
        public string AggregatorUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("publisher")]
        public string Publisher { get; set; }

        [JsonProperty("publisher_url")]
        public string PublisherUrl { get; set; }

        [JsonProperty("social_rank")]
        public double SocialRank { get; set; }

        [JsonProperty("ingredients")]
        public string[] Ingredients { get; set; }
    }
}
