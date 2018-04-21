using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using AutoFixture;
using System.Linq;
using SafeToNet.Prototype.Core.Domain;

namespace SafeToNet.Prototype.ExternalClients.Tests
{
    public class Food2ForkClientTests
    {
        [Fact, Trait("Category", "Integration")]
        public async Task Integration_TestSearch()
        {
            var logger = new Mock<ILogger<Food2Fork.Food2ForkClient>>(MockBehavior.Loose);

            var config = new Core.Configuration.Food2ForkConfiguration
            {
                ApiKey = System.Environment.GetEnvironmentVariable("ApiKey"),
                BaseUrl = @"https://food2fork.com"
            };
            var options = new Mock<IOptionsSnapshot<Core.Configuration.Food2ForkConfiguration>>(MockBehavior.Strict);
            options.Setup(o => o.Value).Returns(() => config);

            var sut = new Food2Fork.Food2ForkClient(logger.Object, options.Object);

            var results = await sut.Search(new[] { "tomato", "rice" }, SearchSorting.Rating);

            Assert.NotNull(results);
            Assert.True(results.Count > 0);

            foreach (var recipe in results.Recipes)
            {
                Assert.NotNull(recipe.Title);
                Assert.NotNull(recipe.ImageUrl);
                Assert.NotNull(recipe.AggregatorUrl);
            }
        }

        [Fact, Trait("Category", "Integration")]
        public async Task Integration_TestGet()
        {
            var logger = new Mock<ILogger<Food2Fork.Food2ForkClient>>(MockBehavior.Loose);

            var config = new Core.Configuration.Food2ForkConfiguration
            {
                ApiKey = @"cb2be9c88acfcfe5ee6677af72630d44",
                BaseUrl = @"https://food2fork.com"
            };
            var options = new Mock<IOptionsSnapshot<Core.Configuration.Food2ForkConfiguration>>(MockBehavior.Strict);
            options.Setup(o => o.Value).Returns(() => config);

            var sut = new Food2Fork.Food2ForkClient(logger.Object, options.Object);

            var results = await sut.Get("39635");

            Assert.NotNull(results);
            Assert.Equal("39635", results.RecipeId);
        }
    }
}
