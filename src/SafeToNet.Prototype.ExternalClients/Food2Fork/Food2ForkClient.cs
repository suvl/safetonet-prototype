using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nelibur.ObjectMapper;
using SafeToNet.Prototype.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SafeToNet.Prototype.ExternalClients.Food2Fork
{
    public class Food2ForkClient : Core.Interfaces.IRecipeAggregatorClient
    {
        private readonly IOptions<Core.Configuration.Food2ForkConfiguration> _configuration;
        private readonly IFlurlClient _client;
        private readonly ILogger _logger;

        static Food2ForkClient()
        {
            TinyMapper.Bind<Recipe, Core.Domain.Recipe>();
            TinyMapper.Bind<RecipeSearchResult, Core.Domain.RecipeSearchResult>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Food2ForkClient"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException">
        /// logger
        /// or
        /// configuration
        /// </exception>
        public Food2ForkClient(ILogger<Food2ForkClient> logger, IOptions<Core.Configuration.Food2ForkConfiguration> configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _client = new FlurlClient(_configuration.Value.BaseUrl);

            _logger.LogInformation("Food2ForkClient .ctor");
        }

        /// <summary>
        /// Gets a recipe by the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Task&lt;Domain.Recipe&gt;.</returns>
        public async Task<Core.Domain.Recipe> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            using (_logger.BeginScope($"Food2ForClient.Get {id}"))
            {
                var uri = $"{_configuration.Value.BaseUrl}{_configuration.Value.GetResource}"
                    .Replace("{key}", _configuration.Value.ApiKey)
                    .Replace("{id}", id);

                var result = await uri.GetStringAsync();

                _logger.LogDebug("GET {uri} = {result}", uri, result);

                var recipe = Newtonsoft.Json.JsonConvert.DeserializeObject<RecipeGetResult>(result);

                return TinyMapper.Map<Core.Domain.Recipe>(recipe.Recipe);
            }
        }

        /// <summary>
        /// Searches for the specified ingredients.
        /// </summary>
        /// <param name="ingredients">The ingredients.</param>
        /// <param name="sorting">The sorting.</param>
        /// <returns>Task&lt;Domain.RecipeSearchResult&gt;.</returns>
        public Task<Core.Domain.RecipeSearchResult> Search(string[] ingredients, SearchSorting sorting)
        {
            using (_logger.BeginScope("Food2ForkClient.Search"))
            {
                var uri = $"{_configuration.Value.BaseUrl}{_configuration.Value.SearchResource}"
                    .Replace("{key}", _configuration.Value.ApiKey)
                    .Replace("{sorting}", GetSortValue(sorting));

                var request = uri.WithClient(_client);

                if (ingredients?.Length > 0)
                {
                    var query = string.Join(",", ingredients);
                    request = request.SetQueryParam(_configuration.Value.SearchQueryParameter, query);
                }

                return request
                    .GetJsonAsync<RecipeSearchResult>()
                    .ContinueWith(t =>
                    {
                        _logger.LogDebug("Result {result}", t.Result);
                        return TinyMapper.Map<Core.Domain.RecipeSearchResult>(t.Result);
                    });
            }
        }

        private static string GetSortValue(SearchSorting sorting)
        {
            switch (sorting)
            {
                case SearchSorting.Rating:
                    return "r";
                case SearchSorting.Trending:
                    return "t";
                default:
                    throw new ArgumentOutOfRangeException(nameof(sorting), sorting, null);
            }
        }
    }
}
