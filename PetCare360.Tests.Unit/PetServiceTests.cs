using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using PetCare360.DTOs.Requests;
using PetCare360.Enums;
using PetCare360.Exceptions;
using PetCare360.Models;
using PetCare360.Repositories.Interfaces;
using PetCare360.Services;

namespace PetCare360.Tests.Unit;

public class PetServiceTests
{
    private readonly Mock<IPetRepository> _petRepositoryMock;
    private readonly Mock<IDeviceRepository> _deviceRepositoryMock;
    private readonly Mock<ISensorDataRepository> _sensorDataRepositoryMock;
    private readonly Mock<IAlertRepository> _alertRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;

    private readonly PetMapper _mapper;
    private readonly CurrentUserService _currentUserService;
    private readonly PetService _petService;

    public PetServiceTests()
    {
        _petRepositoryMock = new Mock<IPetRepository>();
        _deviceRepositoryMock = new Mock<IDeviceRepository>();
        _sensorDataRepositoryMock = new Mock<ISensorDataRepository>();
        _alertRepositoryMock = new Mock<IAlertRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _mapper = new PetMapper();

        _currentUserService = new CurrentUserService(
            _httpContextAccessorMock.Object,
            _userRepositoryMock.Object);

        _petService = new PetService(
            _petRepositoryMock.Object,
            _deviceRepositoryMock.Object,
            _sensorDataRepositoryMock.Object,
            _alertRepositoryMock.Object,
            _currentUserService,
            _mapper);
    }

    private void ConfigureAuthenticatedUser(string email)
    {
        var context = new DefaultHttpContext();

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Email, email)
        };

        context.User = new ClaimsPrincipal(
            new ClaimsIdentity(
                claims,
                "TestAuthentication"));

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(context);
    }

    [Fact]
    public async Task FindOwnedPetAsync_PetPertenceAoUsuario_RetornaPet()
    {
        // Arrange
        const long petId = 1;
        const string email = "usuario@email.com";

        ConfigureAuthenticatedUser(email);

        var pet = new Pet
        {
            Id = petId,
            UserId = 10,
            Name = "Rex",
            Age = 5,
            Weight = 20.5m,
            Breed = "Labrador",
            DeviceId = "DEVICE-001",
            CreatedAt = DateTimeOffset.UtcNow
        };

        _petRepositoryMock
            .Setup(x => x.FindByIdAndUserEmailAsync(petId, email))
            .ReturnsAsync(pet);

        // Act
        var result = await _petService.FindOwnedPetAsync(petId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(petId, result.Id);
        Assert.Equal("Rex", result.Name);

        _petRepositoryMock.Verify(
            x => x.FindByIdAndUserEmailAsync(petId, email),
            Times.Once);
    }

    [Fact]
    public async Task FindOwnedPetAsync_PetNaoExiste_LancaNotFoundException()
    {
        // Arrange
        const long petId = 999;
        const string email = "usuario@email.com";

        ConfigureAuthenticatedUser(email);

        _petRepositoryMock
            .Setup(x => x.FindByIdAndUserEmailAsync(petId, email))
            .ReturnsAsync((Pet?)null);

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _petService.FindOwnedPetAsync(petId));

        // Assert
        Assert.Equal("Pet não encontrado.", exception.Message);

        _petRepositoryMock.Verify(
            x => x.FindByIdAndUserEmailAsync(petId, email),
            Times.Once);
    }

    [Fact]
    public async Task FindOwnedPetAsync_UsuarioNaoAutenticado_LancaUnauthorizedException()
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.User = new ClaimsPrincipal(
            new ClaimsIdentity());

        _httpContextAccessorMock
            .Setup(x => x.HttpContext)
            .Returns(context);

        // Act
        var exception = await Assert.ThrowsAsync<UnauthorizedException>(
            () => _petService.FindOwnedPetAsync(1));

        // Assert
        Assert.Equal("Usuário não autenticado.", exception.Message);

        _petRepositoryMock.Verify(
            x => x.FindByIdAndUserEmailAsync(
                It.IsAny<long>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_DeviceJaVinculado_LancaConflictException()
    {
        // Arrange
        const string email = "usuario@email.com";

        ConfigureAuthenticatedUser(email);

        var user = new AppUser
        {
            Id = 1,
            Name = "Usuário Teste",
            Email = email,
            PasswordHash = "hash"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        _deviceRepositoryMock
            .Setup(x => x.ExistsByDeviceIdAsync("DEVICE-001"))
            .ReturnsAsync(true);

        var request = CreatePetRequest("DEVICE-001");

        // Act
        var exception = await Assert.ThrowsAsync<ConflictException>(
            () => _petService.CreateAsync(request));

        // Assert
        Assert.Equal(
            "Este device já está vinculado a outro pet.",
            exception.Message);

        _deviceRepositoryMock.Verify(
            x => x.ExistsByDeviceIdAsync("DEVICE-001"),
            Times.Once);

        _petRepositoryMock.Verify(
            x => x.SaveAsync(It.IsAny<Pet>()),
            Times.Never);

        _sensorDataRepositoryMock.Verify(
            x => x.SaveAsync(It.IsAny<SensorData>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_PetPertenceAoUsuario_RemovePet()
    {
        // Arrange
        const long petId = 1;
        const string email = "usuario@email.com";

        ConfigureAuthenticatedUser(email);

        var pet = new Pet
        {
            Id = petId,
            UserId = 1,
            Name = "Rex",
            Age = 5,
            Weight = 20m,
            Breed = "Labrador",
            DeviceId = "DEVICE-001",
            CreatedAt = DateTimeOffset.UtcNow
        };

        _petRepositoryMock
            .Setup(x => x.FindByIdAndUserEmailAsync(petId, email))
            .ReturnsAsync(pet);

        _petRepositoryMock
            .Setup(x => x.DeleteAsync(pet))
            .Returns(Task.CompletedTask);

        // Act
        await _petService.DeleteAsync(petId);

        // Assert
        _petRepositoryMock.Verify(
            x => x.FindByIdAndUserEmailAsync(petId, email),
            Times.Once);

        _petRepositoryMock.Verify(
            x => x.DeleteAsync(pet),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_PetNaoExiste_LancaNotFoundException()
    {
        // Arrange
        const long petId = 999;
        const string email = "usuario@email.com";

        ConfigureAuthenticatedUser(email);

        _petRepositoryMock
            .Setup(x => x.FindByIdAndUserEmailAsync(petId, email))
            .ReturnsAsync((Pet?)null);

        // Act
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _petService.DeleteAsync(petId));

        // Assert
        Assert.Equal("Pet não encontrado.", exception.Message);

        _petRepositoryMock.Verify(
            x => x.DeleteAsync(It.IsAny<Pet>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_DeviceDisponivel_CriaPetEProcessaSensorInicial()
    {
        // Arrange
        const string email = "usuario@email.com";

        ConfigureAuthenticatedUser(email);

        var user = new AppUser
        {
            Id = 1,
            Name = "Usuário Teste",
            Email = email,
            PasswordHash = "hash"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        _deviceRepositoryMock
            .Setup(x => x.ExistsByDeviceIdAsync("DEVICE-001"))
            .ReturnsAsync(false);

        var savedPet = new Pet
        {
            Id = 10,
            UserId = user.Id,
            User = user,
            Name = "Rex",
            Age = 5,
            Weight = 20m,
            Breed = "Labrador",
            DeviceId = "DEVICE-001",
            CreatedAt = DateTimeOffset.UtcNow
        };

        _petRepositoryMock
            .Setup(x => x.SaveAsync(It.IsAny<Pet>()))
            .ReturnsAsync(savedPet);

        _sensorDataRepositoryMock
            .Setup(x => x.FindFirstByDevicePetIdOrderByTimestampDescAsync(
                It.IsAny<long>()))
            .ReturnsAsync((SensorData?)null);

        _sensorDataRepositoryMock
            .Setup(x => x.SaveAsync(It.IsAny<SensorData>()))
            .ReturnsAsync((SensorData data) => data);

        _deviceRepositoryMock
            .Setup(x => x.SaveAsync(It.IsAny<Device>()))
            .ReturnsAsync((Device device) => device);

        _alertRepositoryMock
            .Setup(x => x.SaveAllAsync(It.IsAny<IEnumerable<Alert>>()))
            .ReturnsAsync((IEnumerable<Alert> alerts) => alerts);

        var request = CreatePetRequest("DEVICE-001");

        // Act
        var result = await _petService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(savedPet.Id, result.Id);
        Assert.Equal("Rex", result.Name);
        Assert.Equal("DEVICE-001", result.DeviceId);

        _deviceRepositoryMock.Verify(
            x => x.ExistsByDeviceIdAsync("DEVICE-001"),
            Times.Once);

        _petRepositoryMock.Verify(
            x => x.SaveAsync(It.Is<Pet>(p =>
                p.Name == "Rex" &&
                p.DeviceId == "DEVICE-001")),
            Times.Once);

        _sensorDataRepositoryMock.Verify(
            x => x.SaveAsync(It.IsAny<SensorData>()),
            Times.Once);

        _deviceRepositoryMock.Verify(
            x => x.SaveAsync(It.IsAny<Device>()),
            Times.Once);
    }

    private static PetRequest CreatePetRequest(string deviceId)
    {
        var sensorData = new InitialSensorDataRequest(
            DateTimeOffset.UtcNow,
            38.5m,
            100,
            70,
            -23.5505m,
            -46.6333m,
            90);

        return new PetRequest(
            "Rex",
            5,
            20m,
            "Labrador",
            deviceId,
            sensorData);
    }
}