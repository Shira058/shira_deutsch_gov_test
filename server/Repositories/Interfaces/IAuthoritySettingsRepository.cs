using RA_MuniAnnouncements.Models;

namespace RA_MuniAnnouncements.Repositories.Interfaces;

/// <summary>Reads per-authority site settings.</summary>
public interface IAuthoritySettingsRepository
{
    /// <summary>Returns the authority's settings, or <c>null</c> if the authority has none.</summary>
    /// <exception cref="Exceptions.UpstreamUnavailableException">The settings store is unreachable.</exception>
    Task<AuthoritySiteSettings?> GetByAuthorityCodeAsync(string authorityCode, CancellationToken ct);
}
