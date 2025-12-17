using CommomTestUtilities.Requests;
using CommomTestUtilities.Tokens;
using FluentAssertions;
using MyRecipeBook.Exception;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Test.InlineData;

namespace WebApi.Test.Recipe.Register;
public class RegisterRecipeTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "recipe";
    private readonly Guid _userIdentifier;
    public RegisterRecipeTest(CustomWebApplicationFactory factory) : base(factory) 
    {
        _userIdentifier = factory.GetUserIdentifier();
    }
    [Fact]
    public async Task Success()
    {
        var request = RequestRecipeJsonBuilder.Build();

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoPostFormData(method: METHOD, request: request, token: token);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("title").GetString()
            .Should().Be(request.Title);

        responseData.RootElement.GetProperty("id").GetString().Should().NotBeNullOrWhiteSpace();
    }
    [Theory]
    [ClassData(typeof(CultureInlineDataTest))]
    public async Task Error_Title_Empty(string culture)
    {
        var request = RequestRecipeJsonBuilder.Build();
        request.Title = string.Empty;

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await DoPostFormData(method: METHOD, request: request, token: token, culture: culture);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("RECIPE_TITLE_EMPTY", new CultureInfo(culture));

        errors.Should().HaveCount(1).And.Contain(error => error.GetString()!.Equals(expectedMessage));
    }
}
