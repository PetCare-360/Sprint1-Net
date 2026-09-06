using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PetCare360.Tests.Integration;

public class PetCareWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.UseSetting(
            "Testing:DatabaseName",
            $"PetCare360IntegrationTests-{Guid.NewGuid()}");
    }
}