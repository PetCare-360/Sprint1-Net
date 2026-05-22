using PetCare360.DTOs.Requests;
using PetCare360.DTOs.Responses;
using PetCare360.Enums;

namespace PetCare360.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(AuthRequest request);
}

public interface IPetService
{
    Task<PetPageResponse> ListAsync(int page, int size);
    Task<IEnumerable<PetResponse>> ListAllAsync();
    Task<PetResponse> FindAsync(long id);
    Task<PetHealthStatusResponse> HealthStatusAsync(long id);
    Task<IEnumerable<QuickAlertPetResponse>> QuickAlertsAsync();
    Task<ActivitySummaryResponse> ActivitySummaryAsync(long id);
    Task<PetResponse> CreateAsync(PetRequest request);
    Task<PetResponse> UpdateAsync(long id, PetRequest request);
    Task DeleteAsync(long id);
}

public interface IMonitoringService
{
    Task<PetSummaryResponse> SummaryAsync(long petId);
    Task<PagedResponse<SensorDataResponse>> MonitoringAsync(long petId, int page, int size);
    Task<PagedResponse<SensorDataResponse>> ActivityAsync(long petId, int page, int size);
    Task<SensorDataResponse?> LocationAsync(long petId);
    Task<PagedResponse<AlertResponse>> AlertsAsync(long petId, int page, int size);
}

public interface IIotProcessingService
{
    Task<IotDataResponse> ProcessAsync(IotDataRequest request);
}