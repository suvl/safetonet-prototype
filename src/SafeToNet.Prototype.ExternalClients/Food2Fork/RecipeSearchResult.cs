using System.Collections.Generic;

namespace SafeToNet.Prototype.ExternalClients.Food2Fork
{
    /// <summary>
    /// Class RecipeSearchResult.
    /// </summary>
    public class RecipeSearchResult
    {
        public int Count { get; set; }

        public List<Recipe> Recipes { get; set; }
    }
}
