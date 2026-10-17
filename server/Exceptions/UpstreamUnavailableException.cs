namespace RA_MuniAnnouncements.Exceptions;

/// <summary>
/// A data source (CMS or MongoDB) could not be reached or returned an invalid response. Mapped to 503.
/// Wraps the driver-specific exception so services and controllers stay unaware of HTTP and Mongo types.
/// </summary>
public sealed class UpstreamUnavailableException(string upstreamName, Exception innerException)
    : Exception($"Upstream '{upstreamName}' is unavailable.", innerException)
{
    public string UpstreamName { get; } = upstreamName;
}
