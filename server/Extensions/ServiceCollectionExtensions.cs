using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using RA_MuniAnnouncements.Models;
using RA_MuniAnnouncements.Options;
using RA_MuniAnnouncements.Repositories;
using RA_MuniAnnouncements.Repositories.Interfaces;

namespace RA_MuniAnnouncements.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the repositories. <c>DataSources:UseMocks</c> selects hard-coded data or the real CMS and MongoDB,
    /// so switching between them is a configuration change only.
    /// </summary>
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("DataSources:UseMocks"))
        {
            services.AddSingleton<IAnnouncementsRepository, MockAnnouncementsRepository>();
            services.AddSingleton<IAuthoritySettingsRepository, MockAuthoritySettingsRepository>();
            return services;
        }

        var cms = GetRequiredOptions<CmsOptions>(configuration, CmsOptions.SectionName);
        services.AddHttpClient<IAnnouncementsRepository, CmsAnnouncementsRepository>(client =>
        {
            client.BaseAddress = cms.BaseUrl;
            client.Timeout = cms.Timeout;
        });

        var mongo = GetRequiredOptions<MongoOptions>(configuration, MongoOptions.SectionName);
        RegisterMongoConventions();
        // MongoClient is thread-safe and owns the connection pool, so a single instance serves the whole app.
        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongo.ConnectionString));
        services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>()
            .GetDatabase(mongo.Database)
            .GetCollection<AuthoritySiteSettings>(MongoAuthoritySettingsRepository.CollectionName));
        services.AddSingleton<IAuthoritySettingsRepository, MongoAuthoritySettingsRepository>();

        return services;
    }

    private static T GetRequiredOptions<T>(IConfiguration configuration, string sectionName) =>
        configuration.GetSection(sectionName).Get<T>()
        ?? throw new InvalidOperationException($"Configuration section '{sectionName}' is required when DataSources:UseMocks is false.");

    // Maps the stored camelCase fields and ignores Mongo's _id, without putting Mongo attributes on the models.
    private static void RegisterMongoConventions() =>
        ConventionRegistry.Register(
            "AuthoritySiteSettings",
            new ConventionPack { new CamelCaseElementNameConvention(), new IgnoreExtraElementsConvention(true) },
            type => type == typeof(AuthoritySiteSettings));
}
