using System.Net.Http.Json;
using System.Text.Json;
using RA_MuniAnnouncements.Exceptions;
using RA_MuniAnnouncements.Models;
using RA_MuniAnnouncements.Repositories.Interfaces;

namespace RA_MuniAnnouncements.Repositories;

/// <summary>Reads announcements from the CMS over HTTP. Base address and timeout are set at registration.</summary>
public sealed class CmsAnnouncementsRepository(HttpClient httpClient) : IAnnouncementsRepository
{
    public async Task<IReadOnlyList<CmsAnnouncement>> GetAnnouncementsAsync(string authorityCode, CancellationToken ct)
    {
        // Escaped so the code can never alter the URL, even if a caller skips validation.
        var requestUri = $"api/content?authority={Uri.EscapeDataString(authorityCode)}";

        try
        {
            using var response = await httpClient.GetAsync(requestUri, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<CmsAnnouncement>>(ct) ?? [];
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException
                                   // A timeout surfaces as a cancellation; a caller abort is not an upstream failure.
                                   || (ex is TaskCanceledException && !ct.IsCancellationRequested))
        {
            throw new UpstreamUnavailableException("CMS", ex);
        }
    }
}
