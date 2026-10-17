using System.Text.Encodings.Web;
using System.Text.Unicode;
using RA_MuniAnnouncements.ErrorHandling;
using RA_MuniAnnouncements.Extensions;
using RA_MuniAnnouncements.Options;
using RA_MuniAnnouncements.Services;
using RA_MuniAnnouncements.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    // Emit Hebrew as-is instead of \uXXXX escapes; HTML-sensitive characters such as < and > stay escaped.
    .AddJsonOptions(options => options.JsonSerializerOptions.Encoder =
        JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Hebrew));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.Configure<NormalizationOptions>(builder.Configuration.GetSection(NormalizationOptions.SectionName));
builder.Services.AddSingleton<IHtmlTextExtractor, HtmlTextExtractor>();
builder.Services.AddSingleton<IAnnouncementNormalizer, AnnouncementNormalizer>();
builder.Services.AddScoped<IAnnouncementsService, AnnouncementsService>();
builder.Services.AddRepositories(builder.Configuration);   // Mock or real, by DataSources:UseMocks

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(allowedOrigins).WithMethods(HttpMethods.Get)));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();

app.Run();
