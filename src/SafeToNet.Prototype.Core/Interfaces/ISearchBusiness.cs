using System.Threading.Tasks;
using SafeToNet.Prototype.Core.Domain;

namespace SafeToNet.Prototype.Core.Interfaces
{
    public interface ISearchBusiness
    {
        /// <summary>
        /// Searches with speech.
        /// </summary>
        /// <param name="speech">The speech.</param>
        /// <returns>Task&lt;RecipeSearchResult&gt;.</returns>
        Task<RecipeSearchResult> SearchWithSpeech(byte[] speech);

        /// <summary>
        /// Searches with text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Task&lt;RecipeSearchResult&gt;.</returns>
        Task<RecipeSearchResult> SearchWithText(string text);
    }
}