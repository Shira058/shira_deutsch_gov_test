using RA_MuniAnnouncements.DTOs;
using RA_MuniAnnouncements.Exceptions;
using RA_MuniAnnouncements.Repositories.Interfaces;
using RA_MuniAnnouncements.Services.Interfaces;

namespace RA_MuniAnnouncements.Services;

public sealed class AnnouncementsService(
    IAnnouncementsRepository announcementsRepository,
    IAuthoritySettingsRepository settingsRepository,
    IAnnouncementNormalizer normalizer,
    TimeProvider timeProvider,
    ILogger<AnnouncementsService> logger) : IAnnouncementsService
{
    public async Task<AnnouncementsResponseDto> GetActiveAnnouncementsAsync(string authorityCode, CancellationToken ct)
    {
        // The two sources are independent, so fetching them concurrently costs one round-trip instead of two.
        var announcementsTask = announcementsRepository.GetAnnouncementsAsync(authorityCode, ct);
        var settingsTask = settingsRepository.GetByAuthorityCodeAsync(authorityCode, ct);
        await Task.WhenAll(announcementsTask, settingsTask);

        var settings = await settingsTask ?? throw new AuthorityNotFoundException(authorityCode);
        var raw = await announcementsTask;
        var response = normalizer.Normalize(settings, raw, timeProvider.GetUtcNow());

        logger.LogInformation(
            "Returned {ActiveCount} of {RawCount} announcements for authority {AuthorityCode}",
            response.Announcements.Count, raw.Count, authorityCode);

        return response;
    }
}
