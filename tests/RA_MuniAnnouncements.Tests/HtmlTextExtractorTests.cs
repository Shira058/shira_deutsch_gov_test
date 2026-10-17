using RA_MuniAnnouncements.Services;

namespace RA_MuniAnnouncements.Tests;

public class HtmlTextExtractorTests
{
    private const int MaxLength = 150;

    private readonly HtmlTextExtractor _extractor = new();

    [Fact]
    public void ToPlainSummary_ExtractsTextAndTruncatesTo150()
    {
        // "Water & Sewage" plus 30 words of 8 characters: 284 characters of plain text.
        var words = Enumerable.Range(1, 30).Select(i => $"notice{i:00}").ToArray();
        var html = $"<h2>Water&nbsp;&amp;&nbsp;Sewage</h2><p>{string.Join("\n  ", words)}</p>";

        var summary = _extractor.ToPlainSummary(html, MaxLength);

        // Word 15 would end at 149, past the 147 left for "...", so the cut falls after word 14.
        Assert.Equal($"Water & Sewage {string.Join(' ', words[..14])}...", summary);
        Assert.True(summary.Length <= MaxLength);
        Assert.Equal("Short text", _extractor.ToPlainSummary("<p>Short   <b>text</b></p>", MaxLength));
    }

    [Theory]
    [InlineData("<p>Hello</p><script>alert('xss')</script>")]
    [InlineData("<img src=x onerror=\"alert('xss')\">Hello")]
    [InlineData("<a href=\"javascript:alert('xss')\">Hello</a>")]
    [InlineData("<iframe src=\"javascript:alert('xss')\"><script>alert('xss')</script></iframe>Hello")]
    [InlineData("<style>body { background: url('javascript:alert(1)') }</style>Hello")]
    public void ToPlainSummary_StripsXssPayloads(string html)
    {
        // Only the visible text survives: no tags, attributes or script code.
        Assert.Equal("Hello", _extractor.ToPlainSummary(html, MaxLength));
    }
}
