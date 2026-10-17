using MongoDB.Driver;
using RA_MuniAnnouncements.Exceptions;
using RA_MuniAnnouncements.Models;
using RA_MuniAnnouncements.Repositories.Interfaces;

namespace RA_MuniAnnouncements.Repositories;

public sealed class MongoAuthoritySettingsRepository(IMongoCollection<AuthoritySiteSettings> collection)
    : IAuthoritySettingsRepository
{
    public const string CollectionName = "AuthoritySiteSettings";

    public async Task<AuthoritySiteSettings?> GetByAuthorityCodeAsync(string authorityCode, CancellationToken ct)
    {
        // A typed filter is sent as a BSON value, never parsed as query text, so it cannot be injected.
        var filter = Builders<AuthoritySiteSettings>.Filter.Eq(s => s.AuthorityCode, authorityCode);

        try
        {
            return await collection.Find(filter).FirstOrDefaultAsync(ct);
        }
        // An unreachable server surfaces as a TimeoutException from server selection, not as a MongoException.
        catch (Exception ex) when (ex is MongoException or TimeoutException)
        {
            throw new UpstreamUnavailableException("MongoDB", ex);
        }
    }
}
