using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SafeToNet.Prototype.Core.Configuration;
using SafeToNet.Prototype.Core.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;
using WitAi.DotNet.Api.Models.Response;

namespace SafeToNet.Prototype.ExternalClients.WitAi
{

    /// <inheritdoc />
    /// <summary>
    /// NLP client for wit.ai
    /// </summary>
    public class WitAiClient : Core.Interfaces.INlpClient
    {
        private readonly IOptions<WitAiConfiguration> _configSnapshot;
        private readonly ILogger _logger;
        private readonly FlurlClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="WitAiClient" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException">
        /// logger or
        /// configuration or 
        /// handler
        /// </exception>
        public WitAiClient(ILogger<WitAiClient> logger, IOptions<WitAiConfiguration> configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configSnapshot = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _client = new FlurlClient(_configSnapshot.Value.BaseUrl);
            _logger.LogDebug("WitAiClient .ctor");
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

                var responseJson = await uri
                    .WithClient(_client)
                    .WithOAuthBearerToken(_configSnapshot.Value.ApiKey)
                    .GetStringAsync();

                _logger.LogDebug("Response JSON: {json}", responseJson);

                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<ParseWitMessageResponse>(responseJson);

                _logger.LogDebug("Response ErrorMessage:{msg}", response.ErrorMessage);

                var result = new ParseResult
                {
                    ErrorMessage = response.ErrorMessage
                };

                if (response.Entities != null)
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

        /// <summary>
        /// Parses the speech.
        /// </summary>
        /// <param name="speech">The speech.</param>
        /// <returns>Task&lt;ParseResult&gt;.</returns>
        /// <exception cref="ArgumentNullException">speech - No speech data given.</exception>
        public async Task<ParseResult> ParseSpeech(byte[] speech)
        {
            if (speech == null || speech.Length == 0)
                throw new ArgumentNullException(nameof(speech), "No speech data given.");

            using (_logger.BeginScope("WitAiClient.ParseSpeech"))
            {
                var uri = $"{_configSnapshot.Value.BaseUrl}{_configSnapshot.Value.SpeechResource}"
                    .Replace("{version}", _configSnapshot.Value.ApiVersion);

                _logger.LogDebug($"POST {uri} with {speech.Length} bytes");

                using (var content = new System.Net.Http.ByteArrayContent(speech))
                {
                    content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("audio/wav");

                    using (var response = await uri
                        .WithClient(_client)
                        .WithOAuthBearerToken(_configSnapshot.Value.ApiKey)
                        .PostAsync(content))
                    {
                        response.EnsureSuccessStatusCode();

                        var data = await response.Content.ReadAsStringAsync();

                        _logger.LogDebug($"response data: {data}");

                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ParseWitMessageResponse>(data);

                        var ret = new ParseResult { ErrorMessage = result.ErrorMessage };

                        if (result.Entities != null)
                        foreach (var entity in result.Entities)
                        {
                            _logger.LogDebug(
                                "ParsedEntity: {name} {values}",
                                entity.Key,
                                string.Join(";", entity.Value.Select(v => v.Value)));

                            var entities = entity.Value.Select(e => new Entity
                            {
                                Value = e.Value.ToString(),
                                Confidence = e.Confidence
                            });

                            ret.Entities.Add(new ParsedEntity
                            {
                                Name = entity.Key,
                                Values = entities.ToList()
                            });
                        }

                        return ret;
                    }
                }
            }
        }
    }
}