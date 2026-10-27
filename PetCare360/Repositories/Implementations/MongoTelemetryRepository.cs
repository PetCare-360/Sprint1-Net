using MongoDB.Driver;
using PetCare360.Models;
using PetCare360.Repositories.Interfaces;

namespace PetCare360.Repositories.Implementations;

public class MongoTelemetryRepository : IMongoTelemetryRepository
{
    private readonly IMongoCollection<MongoTelemetryDocument> _collection;

    public MongoTelemetryRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<MongoTelemetryDocument>(
            "telemetry");
    }

    public async Task SaveAsync(MongoTelemetryDocument document)
    {
        await _collection.InsertOneAsync(document);
    }

    public async Task<List<MongoTelemetryDocument>> GetByPetIdAsync(
        long petId,
        int limit = 100)
    {
        limit = Math.Clamp(limit, 1, 500);

        return await _collection
            .Find(document => document.PetId == petId)
            .SortByDescending(document => document.Timestamp)
            .Limit(limit)
            .ToListAsync();
    }
}
