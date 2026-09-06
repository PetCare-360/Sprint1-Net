using System.Net;
using System.Net.Http.Json;
using PetCare360.DTOs.Requests;
using PetCare360.DTOs.Responses;

namespace PetCare360.Tests.Integration;

public class AuthIntegrationTests
    : IClassFixture<PetCareWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(PetCareWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_UsuarioValido_RetornaCreated()
    {
        // Arrange
        var request = new RegisterRequest(
            "Leonardo Teste",
            $"leonardo-{Guid.NewGuid()}@teste.com",
            "123456");

        // Act
        var response = await _client.PostAsJsonAsync(
            "/auth/register",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var result =
            await response.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.NotEmpty(result.Token);
        Assert.Equal("Bearer", result.Type);
        Assert.Equal(
            request.Email.ToLower(),
            result.User.Email);
    }

    [Fact]
    public async Task Register_EmailDuplicado_RetornaConflict()
    {
        // Arrange
        var email = $"duplicado-{Guid.NewGuid()}@teste.com";

        var request = new RegisterRequest(
            "Usuário Teste",
            email,
            "123456");

        await _client.PostAsJsonAsync(
            "/auth/register",
            request);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/auth/register",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode);
    }

    [Fact]
    public async Task Login_CredenciaisValidas_RetornaOk()
    {
        // Arrange
        var email = $"login-{Guid.NewGuid()}@teste.com";
        const string password = "123456";

        await _client.PostAsJsonAsync(
            "/auth/register",
            new RegisterRequest(
                "Usuário Login",
                email,
                password));

        var request = new AuthRequest(
            email,
            password);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/auth/login",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var result =
            await response.Content.ReadFromJsonAsync<AuthResponse>();

        Assert.NotNull(result);
        Assert.NotEmpty(result.Token);
        Assert.Equal("Bearer", result.Type);
        Assert.Equal(email, result.User.Email);
    }

    [Fact]
    public async Task Login_SenhaIncorreta_RetornaUnauthorized()
    {
        // Arrange
        var email = $"senha-{Guid.NewGuid()}@teste.com";

        await _client.PostAsJsonAsync(
            "/auth/register",
            new RegisterRequest(
                "Usuário Senha",
                email,
                "123456"));

        var request = new AuthRequest(
            email,
            "senha-incorreta");

        // Act
        var response = await _client.PostAsJsonAsync(
            "/auth/login",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task Login_UsuarioInexistente_RetornaUnauthorized()
    {
        // Arrange
        var request = new AuthRequest(
            $"inexistente-{Guid.NewGuid()}@teste.com",
            "123456");

        // Act
        var response = await _client.PostAsJsonAsync(
            "/auth/login",
            request);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }
}