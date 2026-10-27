using PetCare360.Models;

namespace PetCare360.Repositories.Interfaces;

public interface IMongoTelemetryRepository
{
    Task SaveAsync(MongoTelemetryDocument document);

    Task<List<MongoTelemetryDocument>> GetByPetIdAsync(
        long petId,
        int limit = 100);
}
