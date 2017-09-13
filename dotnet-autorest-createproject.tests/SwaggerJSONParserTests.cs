﻿using FluentAssertions;
using Xunit;

namespace ESW.autorest.createProject.tests
{
    // ReSharper disable once InconsistentNaming
    public class SwaggerJSONParserTests
    {
        [Theory, Trait("Category", "Unit")]
        [InlineData("v1", "1")]
        [InlineData("v1.3", "1.3")]
        [InlineData("1", "1")]
        [InlineData("1.2", "1.2")]
        [InlineData("v1.2.2", "1.2.2")]
        [InlineData("1.2.2", "1.2.2")]
        [InlineData("v1.2.2.5", "1.2.2.5")]
        [InlineData("1.2.2.5", "1.2.2.5")]
        public void SanitizeVersion_Passes(string input, string expected)
        {
            SwaggerJsonParser.SanitizeVersion(input).Should().Be(expected);
        }

        [Theory, Trait("Category", "Unit")]
        [InlineData("lorem ipsum", "loremipsum")]
        [InlineData("a/", "a")]
        [InlineData("a?", "a")]
        [InlineData("a:", "a")]
        [InlineData("a&", "a")]
        [InlineData("a\\", "a")]
        [InlineData("a*", "a")]
        [InlineData("a\"", "a")]
        [InlineData("a<", "a")]
        [InlineData("a>", "a")]
        [InlineData("a#", "a")]
        [InlineData("a%", "a")]


        public void SanitizeTitle_Passes(string input, string expected)
        {
            SwaggerJsonParser.SanitizeTitle(input).Should().Be(expected);
        }
    }
}
