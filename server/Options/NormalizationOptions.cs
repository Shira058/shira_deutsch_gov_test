namespace RA_MuniAnnouncements.Options;

public sealed record NormalizationOptions
{
    public const string SectionName = "Normalization";

    /// <summary>Used when an authority's maxItems is missing or not positive.</summary>
    public int DefaultMaxItems { get; init; } = 10;
}
