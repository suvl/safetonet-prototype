using Newtonsoft.Json;

namespace SafeToNet.Prototype.ExternalClients.Food2Fork
{
    public class RecipeGetResult
    {
        [JsonProperty("recipe")]
        public Recipe Recipe { get; set; }
    }
}
