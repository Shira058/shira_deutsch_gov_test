namespace RA_MuniAnnouncements.Options;

public sealed record MongoOptions
{
    public const string SectionName = "Mongo";

    public required string ConnectionString { get; init; }
    public required string Database { get; init; }
}
