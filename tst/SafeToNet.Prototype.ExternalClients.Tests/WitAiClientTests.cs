using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;
using WitAi.DotNet.Api.Models.Response;
using WitAi.DotNet.Api.Models.Domain;
using System.Collections.Generic;
using AutoFixture;
using System.Linq;

namespace SafeToNet.Prototype.ExternalClients.Tests
{
    public class WitAiClientTests
    {

        [Fact, Trait("Category", "Unit")]
        public async Task Test_ParseOk()
        {
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            using (var test = new Flurl.Http.Testing.HttpTest())
            {
                test.RespondWithJson(new ParseWitMessageResponse
                {
                    Text = "text",
                    IsSuccessful = true,
                    MessageId = System.Guid.NewGuid().ToString(),
                    Entities = new Dictionary<string, List<WitParsedEntity>>
                    {
                        { "entity1", new List<WitParsedEntity>(fixture.CreateMany<WitParsedEntity>(5)) },
                        { "entity2", new List<WitParsedEntity>(fixture.CreateMany<WitParsedEntity>(5)) },
                        { "entity3", new List<WitParsedEntity>(fixture.CreateMany<WitParsedEntity>(5)) }
                    }
                });

                var loggerMock = new Mock<ILogger<WitAi.WitAiClient>>(MockBehavior.Loose);
                var config = new Core.Configuration.WitAiConfiguration
                {
                    ApiKey = "someapikey",
                    ApiVersion = "someapiversion",
                    BaseUrl = "http://someurl.fqdn.local"
                };
                var options = new Mock<IOptionsSnapshot<Core.Configuration.WitAiConfiguration>>(MockBehavior.Strict);
                options.Setup(o => o.Value)
                    .Returns(() => config);

                var sut = new WitAi.WitAiClient(loggerMock.Object, options.Object);

                var result = await sut.Parse("test string");

                Assert.NotNull(result);
                Assert.Null(result.ErrorMessage);
                Assert.NotNull(result.Entities);
                Assert.NotEmpty(result.Entities);

                foreach (var entity in result.Entities)
                {
                    Assert.StartsWith("entity", entity.Name);
                    Assert.Equal(5, entity.Values.Count);
                }
            }
        }

        [Fact, Trait("Category", "Unit")]
        public async Task Test_ParseNOk()
        {
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            using (var test = new Flurl.Http.Testing.HttpTest())
            {
                test.RespondWithJson(new ParseWitMessageResponse
                {
                    Text = "text",
                    IsSuccessful = true,
                    MessageId = System.Guid.NewGuid().ToString(),
                    ErrorMessage = "error"
                });

                var loggerMock = new Mock<ILogger<WitAi.WitAiClient>>(MockBehavior.Loose);
                var config = new Core.Configuration.WitAiConfiguration
                {
                    ApiKey = "someapikey",
                    ApiVersion = "someapiversion",
                    BaseUrl = "http://someurl.fqdn.local"
                };
                var options = new Mock<IOptionsSnapshot<Core.Configuration.WitAiConfiguration>>(MockBehavior.Strict);
                options.Setup(o => o.Value)
                    .Returns(() => config);

                var sut = new WitAi.WitAiClient(loggerMock.Object, options.Object);

                var result = await sut.Parse("test string");

                Assert.NotNull(result);
                Assert.NotNull(result.ErrorMessage);
                Assert.Empty(result.Entities);
            }
        }

        [Fact, Trait("Category", "Unit")]
        public async Task Speech_ParseOk()
        {
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                    .ToList()
                    .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            using (var test = new Flurl.Http.Testing.HttpTest())
            {
                test.RespondWithJson(new ParseWitMessageResponse
                {
                    Text = "text",
                    IsSuccessful = true,
                    MessageId = System.Guid.NewGuid().ToString(),
                    Entities = new Dictionary<string, List<WitParsedEntity>>
                    {
                        { "entity1", new List<WitParsedEntity>(fixture.CreateMany<WitParsedEntity>(5)) },
                        { "entity2", new List<WitParsedEntity>(fixture.CreateMany<WitParsedEntity>(5)) },
                        { "entity3", new List<WitParsedEntity>(fixture.CreateMany<WitParsedEntity>(5)) }
                    }
                });

                var loggerMock = new Mock<ILogger<WitAi.WitAiClient>>(MockBehavior.Loose);
                var config = new Core.Configuration.WitAiConfiguration
                {
                    ApiKey = "someapikey",
                    ApiVersion = "someapiversion",
                    BaseUrl = "http://someurl.fqdn.local"
                };
                var options = new Mock<IOptionsSnapshot<Core.Configuration.WitAiConfiguration>>(MockBehavior.Strict);
                options.Setup(o => o.Value)
                                    .Returns(() => config);

                var sut = new WitAi.WitAiClient(loggerMock.Object, options.Object);

                var result = await sut.ParseSpeech(new byte[] { 0x30, 0x60, 0x1f, 0x4f });

                Assert.NotNull(result);
                Assert.Null(result.ErrorMessage);
                Assert.NotNull(result.Entities);
                Assert.NotEmpty(result.Entities);

                foreach (var entity in result.Entities)
                {
                    Assert.StartsWith("entity", entity.Name);
                    Assert.Equal(5, entity.Values.Count);
                }
            }
        }

        [Fact, Trait("Category", "Unit")]
        public async Task Speech_ParseNOk()
        {
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                    .ToList()
                    .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            using (var test = new Flurl.Http.Testing.HttpTest())
            {
                test.RespondWith(@"{""error"":""could not do something""}", 502);

                var loggerMock = new Mock<ILogger<WitAi.WitAiClient>>(MockBehavior.Loose);
                var config = new Core.Configuration.WitAiConfiguration
                {
                    ApiKey = "someapikey",
                    ApiVersion = "someapiversion",
                    BaseUrl = "http://someurl.fqdn.local"
                };
                var options = new Mock<IOptionsSnapshot<Core.Configuration.WitAiConfiguration>>(MockBehavior.Strict);
                options.Setup(o => o.Value)
                                    .Returns(() => config);

                var sut = new WitAi.WitAiClient(loggerMock.Object, options.Object);

                var task = sut.ParseSpeech(new byte[] { 0x30, 0x60, 0x1f, 0x4f });
                await Assert.ThrowsAsync<Flurl.Http.FlurlHttpException>(() => task);
            }
        }
    }
}
