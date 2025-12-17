using CommomTestUtilities.Tokens;
using FluentAssertions;
using System.Net;

namespace WebApi.Test.Dashboard.Get;
public class GetDashboardInvalidTokenTest : MyRecipeBookClassFixture
{
    private const string METHOD = "dashboard";
    public GetDashboardInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }
    [Fact]
    public async Task Error_Token_Invalid() 
    {
        var response = await DoGet(method: METHOD, token: "invalidToken");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task Error_Without_Token()
    {
        var response = await DoGet(method: METHOD, token: string.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task Error_Token_With_User_Not_Found()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var response = await DoGet(method: METHOD, token: token);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
