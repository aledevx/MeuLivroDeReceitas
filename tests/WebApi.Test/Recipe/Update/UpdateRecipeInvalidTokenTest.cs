using CommomTestUtilities.Requests;
using CommomTestUtilities.Tokens;
using FluentAssertions;
using System.Net;

namespace WebApi.Test.Recipe.Update;
public class UpdateRecipeInvalidTokenTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "recipe";
    private readonly string _recipeId;
    public UpdateRecipeInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _recipeId = factory.GetRecipeId();
    }
    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = RequestRecipeJsonBuilder.Build();

        var response = await DoPut(method: $"{METHOD}/{_recipeId}", request: request, token: "invalidToken");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task Error_Without_Token()
    {
        var request = RequestRecipeJsonBuilder.Build();

        var response = await DoPut(method: $"{METHOD}/{_recipeId}", request: request, token: string.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var request = RequestRecipeJsonBuilder.Build();

        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var response = await DoPut(method: $"{METHOD}/{_recipeId}", request: request, token: token);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
