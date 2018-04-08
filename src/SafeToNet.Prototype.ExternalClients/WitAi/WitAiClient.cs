using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WitAi.DotNet.Api;
using WitAi.DotNet.Api.Models.Request;
using WitAi.DotNet.Api.Models.Response;

namespace SafeToNet.Prototype.ExternalClients.WitAi
{
    using Core.Configuration;
    using Core.Domain;

    /// <inheritdoc />
    /// <summary>
    /// NLP client for wit.ai
    /// </summary>
    public class WitAiClient : Core.Interfaces.INlpClient
    {
        private readonly IOptionsSnapshot<WitAiConfiguration> _configSnapshot;
        private readonly ILogger _logger;
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="WitAiClient" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="snapshot">The snapshot.</param>
        /// <param name="handler">The handler.</param>
        /// <exception cref="ArgumentNullException">
        /// logger or
        /// snapshot or 
        /// handler
        /// </exception>
        public WitAiClient(ILogger<WitAiClient> logger, IOptionsSnapshot<WitAiConfiguration> snapshot, HttpMessageHandler handler)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configSnapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
            _client = new HttpClient(handler ?? throw new ArgumentNullException(nameof(handler)));
            _logger.LogDebug("WitAiClient .ctor");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WitAiClient"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="snapshot">The snapshot.</param>
        public WitAiClient(ILogger<WitAiClient> logger, IOptionsSnapshot<WitAiConfiguration> snapshot)
            : this(logger, snapshot, new HttpClientHandler())
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Parses the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task&lt;Domain.ParseResult&gt;.</returns>
        /// <exception cref="T:System.ArgumentNullException">message</exception>
        public async Task<ParseResult> Parse(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            using (_logger.BeginScope("WitAiClient.Parse {0}", message))
            {
                var uri = $"{_configSnapshot.Value.BaseUrl}{_configSnapshot.Value.MessageResource}"
                    .Replace("{version}", _configSnapshot.Value.ApiVersion)
                    .Replace("{query}", Uri.EscapeUriString(message));

                _logger.LogDebug("Requesting GET {uri}", uri);

                var responseJson = await _client.GetStringAsync(uri).ConfigureAwait(false);

                _logger.LogDebug("Response JSON: {json}", responseJson);

                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<ParseWitMessageResponse>(responseJson);

                _logger.LogDebug("Response ErrorMessage:{msg}", response.ErrorMessage);

                var result = new ParseResult
                {
                    ErrorMessage = response.ErrorMessage
                };

                foreach (var responseEntity in response.Entities)
                {
                    _logger.LogDebug(
                        "ParsedEntity: {name} {values}", 
                        responseEntity.Key, 
                        string.Join(";", responseEntity.Value.Select(v => v.Value)));

                    var entities = responseEntity.Value.Select(e => new Entity
                    {
                        Value = e.Value.ToString(),
                        Confidence = e.Confidence
                    });
                    
                    result.Entities.Add(new ParsedEntity
                    {
                        Name = responseEntity.Key,
                        Values = entities.ToList()
                    });
                }

                return result;
            }
        }
    }
}