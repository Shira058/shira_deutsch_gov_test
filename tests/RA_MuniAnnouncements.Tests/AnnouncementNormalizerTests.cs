using Microsoft.Extensions.Options;
using RA_MuniAnnouncements.DTOs;
using RA_MuniAnnouncements.Models;
using RA_MuniAnnouncements.Options;
using RA_MuniAnnouncements.Services;

namespace RA_MuniAnnouncements.Tests;

public class AnnouncementNormalizerTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 15, 12, 0, 0, TimeSpan.Zero);

    private static readonly AuthoritySiteSettings Settings = new()
    {
        AuthorityCode = "6500",
        AuthorityName = "עיריית חדרה",
        EnabledCategories = ["roads", "water", "events"],
        CategoryDisplayNames = new Dictionary<string, string>
        {
            ["roads"] = "תחבורה וכבישים",
            ["water"] = "מים וביוב",
            ["events"] = "אירועים",
        },
        UrgentCategories = ["water"],
        MaxItems = 5,
    };

    // An active, non-urgent announcement. Each test derives its cases from it with 'with'.
    private static readonly CmsAnnouncement Active = new()
    {
        Id = "active",
        AuthorityCode = "6500",
        Title = "Title",
        Body = "<p>Body</p>",
        Category = "roads",
        Status = "Published",
        PublishDate = Now.AddDays(-1),
        ExpiryDate = Now.AddDays(1),
    };

    private readonly AnnouncementNormalizer _normalizer = CreateNormalizer(new NormalizationOptions());

    [Fact]
    public void Normalize_ReturnsOnlyActiveAnnouncementsOfEnabledCategories()
    {
        CmsAnnouncement[] raw =
        [
            Active,
            Active with { Id = "published-now", PublishDate = Now },
            Active with { Id = "draft", Status = "Draft" },
            Active with { Id = "expires-now", ExpiryDate = Now },
            Active with { Id = "future", PublishDate = Now.AddMinutes(1) },
            Active with { Id = "disabled-category", Category = "tenders" },
            Active with { Id = "other-authority", AuthorityCode = "5000" },
        ];

        var result = _normalizer.Normalize(Settings, raw, Now);

        Assert.Equal(new[] { "active", "published-now" }, result.Announcements.Select(a => a.Id).Order());
    }

    [Fact]
    public void Normalize_SortsUrgentFirstThenNewest_AndLimitsToMaxItems()
    {
        CmsAnnouncement[] raw =
        [
            Active with { Id = "oldest", PublishDate = Now.AddDays(-6) },
            Active with { Id = "same-date-b", Category = "events", PublishDate = Now.AddDays(-2) },
            Active with { Id = "old-urgent", Category = "water", PublishDate = Now.AddDays(-5) },
            Active with { Id = "newest", PublishDate = Now.AddHours(-1) },
            Active with { Id = "same-date-a", PublishDate = Now.AddDays(-2) },
            Active with { Id = "new-urgent", Category = "water", PublishDate = Now.AddDays(-3) },
        ];

        var result = _normalizer.Normalize(Settings with { MaxItems = 4 }, raw, Now);

        Assert.Equal(
            new[] { "new-urgent", "old-urgent", "newest", "same-date-a" },
            result.Announcements.Select(a => a.Id));

        // A missing or non-positive maxItems falls back to the configured default.
        var withSmallDefault = CreateNormalizer(new NormalizationOptions { DefaultMaxItems = 2 });
        Assert.Equal(2, withSmallDefault.Normalize(Settings with { MaxItems = 0 }, raw, Now).Announcements.Count);
        Assert.Equal(2, withSmallDefault.Normalize(Settings with { MaxItems = null }, raw, Now).Announcements.Count);
    }

    [Fact]
    public void Normalize_MapsDisplayNameUrgencyAndIsraelDate()
    {
        CmsAnnouncement[] raw =
        [
            // 22:30 UTC is 01:30 the next day in Israel (UTC+3 in October).
            Active with { Id = "urgent", Category = "water", PublishDate = new DateTimeOffset(2026, 10, 1, 22, 30, 0, TimeSpan.Zero) },
            Active with { Id = "regular", Category = "roads", PublishDate = new DateTimeOffset(2026, 10, 1, 8, 0, 0, TimeSpan.Zero) },
        ];

        var result = _normalizer.Normalize(Settings, raw, Now);

        Assert.Equal("6500", result.AuthorityCode);
        Assert.Equal("עיריית חדרה", result.AuthorityName);
        Assert.Equal(
            new[]
            {
                new AnnouncementDto
                {
                    Id = "urgent", Title = "Title", Summary = "Body",
                    CategoryName = "מים וביוב", IsUrgent = true, PublishDate = "02/10/2026",
                },
                new AnnouncementDto
                {
                    Id = "regular", Title = "Title", Summary = "Body",
                    CategoryName = "תחבורה וכבישים", IsUrgent = false, PublishDate = "01/10/2026",
                },
            },
            result.Announcements);
    }

    private static AnnouncementNormalizer CreateNormalizer(NormalizationOptions options) =>
        new(new HtmlTextExtractor(), new OptionsWrapper<NormalizationOptions>(options));
}
