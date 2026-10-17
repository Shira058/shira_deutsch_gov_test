namespace RA_MuniAnnouncements.Models;

/// <summary>A raw announcement as returned by the CMS, before normalization.</summary>
public sealed record CmsAnnouncement
{
    public required string Id { get; init; }
    public required string AuthorityCode { get; init; }
    public required string Title { get; init; }

    /// <summary>Unsanitized HTML authored in the CMS.</summary>
    public string? Body { get; init; }

    public required string Category { get; init; }
    public required string Status { get; init; }
    public required DateTimeOffset PublishDate { get; init; }
    public required DateTimeOffset ExpiryDate { get; init; }
}
