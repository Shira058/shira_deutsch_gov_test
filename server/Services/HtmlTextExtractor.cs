using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using RA_MuniAnnouncements.Services.Interfaces;

namespace RA_MuniAnnouncements.Services;

public sealed partial class HtmlTextExtractor : IHtmlTextExtractor
{
    private const string Ellipsis = "...";

    // Their content is code or raw markup, never visible text. An iframe's content is parsed as raw text,
    // so leaving it would put literal "<script>..." into the summary.
    private const string NonTextElements = "script, style, iframe";

    // TextContent concatenates adjacent blocks ("<p>a</p><p>b</p>" becomes "ab"), so each block gets a trailing space.
    private const string BlockElements =
        "address, article, blockquote, br, dd, div, dt, footer, h1, h2, h3, h4, h5, h6, header, hr, li, p, pre, section, td, th, tr";

    public string ToPlainSummary(string? html, int maxLength)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxLength, Ellipsis.Length);
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;

        // A spec-compliant HTML5 parser reads malformed or hostile markup exactly as a browser would, and never runs scripts.
        using var document = new HtmlParser().ParseDocument(html);
        foreach (var element in document.QuerySelectorAll(NonTextElements))
            element.Remove();
        foreach (var element in document.QuerySelectorAll(BlockElements))
            element.After(document.CreateTextNode(" "));

        var text = Whitespace().Replace(document.Body?.TextContent ?? string.Empty, " ").Trim();
        return Truncate(text, maxLength);
    }

    private static string Truncate(string text, int maxLength)
    {
        if (text.Length <= maxLength) return text;

        var limit = maxLength - Ellipsis.Length;
        // Searching back from the first character that does not fit keeps a word that ends exactly at the limit.
        var lastSpace = text.LastIndexOf(' ', limit);
        // A single word longer than the limit has no boundary to cut at, so it is cut mid-word.
        var cut = lastSpace > 0 ? lastSpace : limit;
        return text[..cut] + Ellipsis;
    }

    // \s also matches non-breaking spaces, which &nbsp; decodes to.
    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();
}
