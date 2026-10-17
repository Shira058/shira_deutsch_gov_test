namespace RA_MuniAnnouncements.Models;

/// <summary>Per-authority site settings, stored in the AuthoritySiteSettings collection.</summary>
public sealed record AuthoritySiteSettings
{
    public required string AuthorityCode { get; init; }
    public required string AuthorityName { get; init; }
    public required IReadOnlyList<string> EnabledCategories { get; init; }

    /// <summary>Display name per category code. Every enabled category must have an entry.</summary>
    public required IReadOnlyDictionary<string, string> CategoryDisplayNames { get; init; }

    public IReadOnlyList<string> UrgentCategories { get; init; } = [];

    /// <summary>Optional; a missing or non-positive value falls back to the configured default.</summary>
    public int? MaxItems { get; init; }
}
