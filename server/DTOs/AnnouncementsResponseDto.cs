namespace RA_MuniAnnouncements.DTOs;

/// <summary>The normalized response returned to the client.</summary>
public sealed record AnnouncementsResponseDto
{
    public required string AuthorityCode { get; init; }
    public required string AuthorityName { get; init; }
    public required IReadOnlyList<AnnouncementDto> Announcements { get; init; }
}
