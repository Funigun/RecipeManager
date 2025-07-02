using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using RecipeManager.API;

namespace RecipeManager.Tests.IntegrationTests.CoreApi.TestFixtures;

public class WebApiFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        
    }
}
