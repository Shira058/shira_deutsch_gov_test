namespace RA_MuniAnnouncements.Exceptions;

/// <summary>No site settings exist for the requested authority. Mapped to 404.</summary>
public sealed class AuthorityNotFoundException(string authorityCode)
    : Exception($"No site settings found for authority '{authorityCode}'.")
{
    public string AuthorityCode { get; } = authorityCode;
}
