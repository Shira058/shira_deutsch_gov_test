using RA_MuniAnnouncements.Models;

namespace RA_MuniAnnouncements.Repositories.Interfaces;

/// <summary>Reads raw announcements from the content management system.</summary>
public interface IAnnouncementsRepository
{
    /// <summary>Returns all raw announcements for the authority, active or not.</summary>
    /// <exception cref="Exceptions.UpstreamUnavailableException">The CMS is unreachable or returned an invalid response.</exception>
    Task<IReadOnlyList<CmsAnnouncement>> GetAnnouncementsAsync(string authorityCode, CancellationToken ct);
}
