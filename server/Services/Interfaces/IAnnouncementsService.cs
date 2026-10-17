using RA_MuniAnnouncements.DTOs;

namespace RA_MuniAnnouncements.Services.Interfaces;

/// <summary>Use case: get the active announcements of an authority, ready for display.</summary>
public interface IAnnouncementsService
{
    /// <exception cref="Exceptions.AuthorityNotFoundException">The authority has no site settings.</exception>
    /// <exception cref="Exceptions.UpstreamUnavailableException">The CMS or the settings store is unavailable.</exception>
    Task<AnnouncementsResponseDto> GetActiveAnnouncementsAsync(string authorityCode, CancellationToken ct);
}
