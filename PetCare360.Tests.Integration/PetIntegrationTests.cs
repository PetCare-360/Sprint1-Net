using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PetCare360.DTOs.Requests;
using PetCare360.DTOs.Responses;

namespace PetCare360.Tests.Integration;

public class PetIntegrationTests
    : IClassFixture<PetCareWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PetIntegrationTests(PetCareWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ListPets_SemAutenticacao_RetornaUnauthorized()
    {
        // Arrange
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/pets");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task ListAllPets_SemAutenticacao_RetornaUnauthorized()
    {
        // Arrange
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/pets/all");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task HealthLive_SemAutenticacao_RetornaHealthy()
    {
        // Arrange
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/health/live");

        // Act
        var response = await _client.SendAsync(request);

        var content =
            await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        Assert.Contains(
            "Healthy",
            content,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Metrics_SemAutenticacao_RetornaOk()
    {
        // Arrange
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/metrics");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    private async Task<string> RegisterAndLoginAsync()
    {
        var email =
            $"pet-{Guid.NewGuid()}@teste.com";

        const string password = "123456";

        var registerResponse =
            await _client.PostAsJsonAsync(
                "/auth/register",
                new RegisterRequest(
                    "Usuário Pet",
                    email,
                    password));

        registerResponse.EnsureSuccessStatusCode();

        var loginResponse =
            await _client.PostAsJsonAsync(
                "/auth/login",
                new AuthRequest(
                    email,
                    password));

        loginResponse.EnsureSuccessStatusCode();

        var auth =
            await loginResponse.Content
                .ReadFromJsonAsync<AuthResponse>();

        return auth!.Token;
    }

    [Fact]
    public async Task ListPets_UsuarioAutenticado_RetornaOk()
    {
        // Arrange
        var token = await RegisterAndLoginAsync();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        // Act
        var response = await _client.GetAsync(
            "/pets");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result =
            await response.Content
                .ReadFromJsonAsync<PetPageResponse>();

        Assert.NotNull(result);
        Assert.Empty(result.Pets);
    }
}