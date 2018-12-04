using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Nip.Blog.Services.Posts.Api;

namespace Nip.Blog.Services.Posts.IntegrationTests
{
    public class BasicTests
    : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public BasicTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/api/v2/BlogPosts")]
        [InlineData("/api/v2/BlogPosts/withtitle/test")]
        [InlineData("/api/v2/BlogPosts/1")]
        [InlineData("/api/v2/BlogPosts/2/comments")]
        public async Task Get_EndpointsReturnOk(string url)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
    }
}	

