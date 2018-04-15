using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SafeToNet.Prototype.Core.Interfaces
{
    /// <summary>
    /// Interface for Recipe aggregator
    /// </summary>
    public interface IRecipeAggregatorClient
    {
        /// <summary>
        /// Searches the specified ingredients.
        /// </summary>
        /// <param name="ingredients">The ingredients.</param>
        /// <returns>Task&lt;Domain.RecipeSearchResult&gt;.</returns>
        Task<Domain.RecipeSearchResult> Search(string[] ingredients);

        /// <summary>
        /// Gets a recipe by the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;Domain.Recipe&gt;.</returns>
        Task<Domain.Recipe> Get(string id);
    }
}
