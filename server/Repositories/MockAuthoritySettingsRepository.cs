using RA_MuniAnnouncements.Models;
using RA_MuniAnnouncements.Repositories.Interfaces;

namespace RA_MuniAnnouncements.Repositories;

/// <summary>Hard-coded settings for three authorities. Any other code (e.g. 9999) has no settings.</summary>
public sealed class MockAuthoritySettingsRepository : IAuthoritySettingsRepository
{
    private static readonly Dictionary<string, AuthoritySiteSettings> Settings = new[]
    {
        new AuthoritySiteSettings
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
        },
        new AuthoritySiteSettings
        {
            AuthorityCode = "5000",
            AuthorityName = "עיריית תל אביב-יפו",
            EnabledCategories = ["roads", "water", "tenders"],
            CategoryDisplayNames = new Dictionary<string, string>
            {
                ["roads"] = "כבישים",
                ["water"] = "מים",
                ["tenders"] = "מכרזים",
            },
            UrgentCategories = ["roads"],
            MaxItems = 3,
        },
        new AuthoritySiteSettings
        {
            AuthorityCode = "7000",
            AuthorityName = "עיריית לוד",
            EnabledCategories = ["roads", "water"],
            CategoryDisplayNames = new Dictionary<string, string>
            {
                ["roads"] = "כבישים",
                ["water"] = "מים",
            },
        },
    }.ToDictionary(s => s.AuthorityCode);

    public Task<AuthoritySiteSettings?> GetByAuthorityCodeAsync(string authorityCode, CancellationToken ct) =>
        Task.FromResult(Settings.GetValueOrDefault(authorityCode));
}
