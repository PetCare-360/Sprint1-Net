using PetCare360.Enums;
using PetCare360.Services;

namespace PetCare360.Tests.Unit;

public class PetServiceStatusTests
{
    [Fact]
    public void CalculateStatus_TemperaturaNormal_RetornaNormal()
    {
        // Arrange
        const decimal temperature = 38.0m;
        const int heartRate = 90;
        const int activityLevel = 50;
        const int battery = 80;

        // Act
        var result = PetService.CalculateStatus(
            temperature,
            heartRate,
            activityLevel,
            battery);

        // Assert
        Assert.Equal(MonitoringStatusEnum.Normal, result);
    }

    [Fact]
    public void CalculateStatus_TemperaturaAcimaDoLimite_RetornaAlerta()
    {
        // Arrange
        const decimal temperature = 39.5m;
        const int heartRate = 90;
        const int activityLevel = 50;
        const int battery = 80;

        // Act
        var result = PetService.CalculateStatus(
            temperature,
            heartRate,
            activityLevel,
            battery);

        // Assert
        Assert.Equal(MonitoringStatusEnum.Alerta, result);
    }

    [Fact]
    public void CalculateStatus_TemperaturaCritica_RetornaCritico()
    {
        // Arrange
        const decimal temperature = 41.0m;
        const int heartRate = 90;
        const int activityLevel = 50;
        const int battery = 80;

        // Act
        var result = PetService.CalculateStatus(
            temperature,
            heartRate,
            activityLevel,
            battery);

        // Assert
        Assert.Equal(MonitoringStatusEnum.Critico, result);
    }

    [Fact]
    public void CalculateStatus_FrequenciaCardiacaAlta_RetornaAlerta()
    {
        // Arrange
        const decimal temperature = 38.0m;
        const int heartRate = 140;
        const int activityLevel = 50;
        const int battery = 80;

        // Act
        var result = PetService.CalculateStatus(
            temperature,
            heartRate,
            activityLevel,
            battery);

        // Assert
        Assert.Equal(MonitoringStatusEnum.Alerta, result);
    }

    [Fact]
    public void CalculateStatus_FrequenciaCardiacaCritica_RetornaCritico()
    {
        // Arrange
        const decimal temperature = 38.0m;
        const int heartRate = 160;
        const int activityLevel = 50;
        const int battery = 80;

        // Act
        var result = PetService.CalculateStatus(
            temperature,
            heartRate,
            activityLevel,
            battery);

        // Assert
        Assert.Equal(MonitoringStatusEnum.Critico, result);
    }

    [Fact]
    public void CalculateStatus_AtividadeBaixa_RetornaAlerta()
    {
        // Arrange
        const decimal temperature = 38.0m;
        const int heartRate = 90;
        const int activityLevel = 10;
        const int battery = 80;

        // Act
        var result = PetService.CalculateStatus(
            temperature,
            heartRate,
            activityLevel,
            battery);

        // Assert
        Assert.Equal(MonitoringStatusEnum.Alerta, result);
    }

    [Fact]
    public void CalculateStatus_BateriaBaixa_RetornaAlerta()
    {
        // Arrange
        const decimal temperature = 38.0m;
        const int heartRate = 90;
        const int activityLevel = 50;
        const int battery = 10;

        // Act
        var result = PetService.CalculateStatus(
            temperature,
            heartRate,
            activityLevel,
            battery);

        // Assert
        Assert.Equal(MonitoringStatusEnum.Alerta, result);
    }

    [Fact]
    public void CalculateStatus_BateriaNula_RetornaNormal()
    {
        // Arrange
        const decimal temperature = 38.0m;
        const int heartRate = 90;
        const int activityLevel = 50;
        int? battery = null;

        // Act
        var result = PetService.CalculateStatus(
            temperature,
            heartRate,
            activityLevel,
            battery);

        // Assert
        Assert.Equal(MonitoringStatusEnum.Normal, result);
    }

    [Fact]
    public void CalculateStatus_TemperaturaCriticaMesmoComOutrosValoresNormais_RetornaCritico()
    {
        // Arrange
        const decimal temperature = 41.5m;
        const int heartRate = 90;
        const int activityLevel = 50;
        const int battery = 80;

        // Act
        var result = PetService.CalculateStatus(
            temperature,
            heartRate,
            activityLevel,
            battery);

        // Assert
        Assert.Equal(MonitoringStatusEnum.Critico, result);
    }
}