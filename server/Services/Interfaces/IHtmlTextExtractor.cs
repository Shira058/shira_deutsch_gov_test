namespace RA_MuniAnnouncements.Services.Interfaces;

/// <summary>Converts untrusted HTML into a short plain-text summary.</summary>
public interface IHtmlTextExtractor
{
    /// <summary>
    /// Returns the visible text of <paramref name="html"/> with entities decoded and whitespace collapsed,
    /// truncated at a word boundary to at most <paramref name="maxLength"/> characters (including "...").
    /// The result never contains markup from the source.
    /// </summary>
    string ToPlainSummary(string? html, int maxLength);
}
