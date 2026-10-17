namespace RA_MuniAnnouncements.DTOs;

/// <summary>A single normalized announcement, ready for display.</summary>
public sealed record AnnouncementDto
{
    public required string Id { get; init; }

    /// <summary>Passed through from the CMS as-is; clients must render it as text.</summary>
    public required string Title { get; init; }

    /// <summary>Plain text, no markup.</summary>
    public required string Summary { get; init; }

    public required string CategoryName { get; init; }
    public required bool IsUrgent { get; init; }

    /// <summary>Publish date in Israel time, formatted dd/MM/yyyy.</summary>
    public required string PublishDate { get; init; }
}
