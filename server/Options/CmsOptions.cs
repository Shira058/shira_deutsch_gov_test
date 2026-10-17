namespace RA_MuniAnnouncements.Options;

public sealed record CmsOptions
{
    public const string SectionName = "Cms";

    public required Uri BaseUrl { get; init; }

    /// <summary>Kept short so a slow CMS turns into a fast 503 instead of piling up requests.</summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(5);
}
