using System;
using System.Collections.Generic;
using System.Text;

namespace SafeToNet.Prototype.Core.Domain
{
    public class RecipeSearchResult
    {
        public int Count { get; set; }

        public List<Recipe> Recipes { get; set; }
    }
}
