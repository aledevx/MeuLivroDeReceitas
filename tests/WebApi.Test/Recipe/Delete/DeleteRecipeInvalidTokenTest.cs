using CommomTestUtilities.IdEncryption;
using CommomTestUtilities.Requests;
using CommomTestUtilities.Tokens;
using FluentAssertions;
using System.Net;

namespace WebApi.Test.Recipe.Delete;
public class DeleteRecipeInvalidTokenTest : MyRecipeBookClassFixture
{
    private readonly string METHOD = "recipe";
    public DeleteRecipeInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }
    [Fact]
    public async Task Error_Token_Invalid()
    {
        var id = IdEncripterBuilder.Build().Encode(1);

        var response = await DoDelete($"{METHOD}/{id}",token: "invalidToken");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task Error_Without_Token()
    {
        var id = IdEncripterBuilder.Build().Encode(1);

        var response = await DoDelete($"{METHOD}/{id}", token: string.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task Error_Token_With_User_NotFound()
    {
        var id = IdEncripterBuilder.Build().Encode(1);

        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var response = await DoDelete($"{METHOD}/{id}", token: string.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
