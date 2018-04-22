using Microsoft.Extensions.Logging;
using SafeToNet.Prototype.Core.Domain;
using SafeToNet.Prototype.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeToNet.Prototype.Business
{
    /// <summary>
    /// Search business layer implementation.
    /// </summary>
    public class SearchBusiness : ISearchBusiness
    {
        private readonly INlpClient _nlpClient;
        private readonly IRecipeAggregatorClient _recipeAggregatorClient;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchBusiness"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="recipeAggregatorClient">The recipe aggregator client.</param>
        /// <param name="nlpClient">The NLP client.</param>
        /// <exception cref="ArgumentNullException">
        /// logger
        /// or
        /// recipeAggregatorClient
        /// or
        /// nlpClient
        /// </exception>
        public SearchBusiness(
            ILogger<SearchBusiness> logger,
            IRecipeAggregatorClient recipeAggregatorClient,
            INlpClient nlpClient)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recipeAggregatorClient = recipeAggregatorClient ?? throw new ArgumentNullException(nameof(recipeAggregatorClient));
            _nlpClient = nlpClient ?? throw new ArgumentNullException(nameof(nlpClient));

            _logger.LogInformation("SearchBusiness .ctor");
        }

        /// <summary>
        /// Searches for recipes with speech data.
        /// </summary>
        /// <param name="speech">The speech.</param>
        /// <returns>Task&lt;RecipeSearchResult&gt;.</returns>
        /// <exception cref="ArgumentNullException">speech - speech is required to search for results</exception>
        public async Task<RecipeSearchResult> SearchWithSpeech(byte[] speech)
        {
            if (speech?.Length == 0)
                throw new ArgumentNullException(nameof(speech), "speech is required to search for results");

            using (_logger.BeginScope("SearchBusiness.SearchWithSpeech"))
            {
                _logger.LogDebug($"speech array: {speech.Length}");

                // natural language processing
                var nlpResults = await _nlpClient.ParseSpeech(speech);
                return await ProcessAndGetRecipes(nlpResults);
            }
        }

        /// <summary>
        /// Searches for recipes with text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Task&lt;RecipeSearchResult&gt;.</returns>
        /// <exception cref="ArgumentNullException">text - a search entry is required to search for results</exception>
        public async Task<RecipeSearchResult> SearchWithText(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text), "a search entry is required to search for results");

            using (_logger.BeginScope("SearchBusiness.SearchWithText {text}", text))
            {
                // natural language processing
                var nlpResults = await _nlpClient.Parse(text);
                return await ProcessAndGetRecipes(nlpResults);
            }
        }

        private Task<RecipeSearchResult> ProcessAndGetRecipes(ParseResult nlpResults)
        {
            // process results
            var processedIntent = ProcessNlpResults(nlpResults);
            var sorting = _sortings[processedIntent.intent];
            // get recipes
            return _recipeAggregatorClient.Search(processedIntent.ingredients, sorting);
        }

        private readonly string[] _conjunctions = {"and", "or"};

        private (string intent, string[] ingredients) ProcessNlpResults(ParseResult nlpResult)
        {
            string intent = "recipe";
            List<string> ingredients = new List<string>();
            foreach (var entity in nlpResult.Entities)
            {
                switch (entity.Name)
                {
                    case "user_intent":
                        intent = entity.Values
                            .OrderByDescending(e => e.Confidence)
                            .First()
                            .Value;
                        break;
                    case "search_query":
                        var queries = entity.Values
                            .SelectMany(v => v.Value.Split(_conjunctions, StringSplitOptions.RemoveEmptyEntries));
                        ingredients.AddRange(queries);
                        break;
                }
            }

            return (intent, ingredients.ToArray());
        }

        private static readonly IDictionary<string, SearchSorting> _sortings = new Dictionary<string, SearchSorting>
        {
            { "recipe", SearchSorting.Rating },
            { "popular", SearchSorting.Trending }
        };
    }
}
