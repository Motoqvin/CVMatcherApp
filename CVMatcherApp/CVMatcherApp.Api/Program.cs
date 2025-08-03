using CVMatcherApp.Api.Data;
using CVMatcherApp.Api.Extensions;
using CVMatcherApp.Api.Jobs;
using CVMatcherApp.Api.Middlewares;
using CVMatcherApp.Api.Options;
using CVMatcherApp.Api.Repositories;
using CVMatcherApp.Api.Repositories.Base;
using CVMatcherApp.Api.Services;
using CVMatcherApp.Api.Services.Base;
using Hangfire;
using Hangfire.PostgreSql;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<JwtOptions>()
    .Configure(options => {
        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

        if(jwtOptions == null) {
            throw new ArgumentNullException(nameof(jwtOptions));
        }

        options.Audience = jwtOptions.Audience ?? throw new ArgumentNullException(nameof(jwtOptions.Audience));
        options.Issuer = jwtOptions.Issuer ?? throw new ArgumentNullException(nameof(jwtOptions.Issuer));

        if(jwtOptions.LifetimeInMinutes == 0) {
            throw new ArgumentNullException(nameof(jwtOptions.LifetimeInMinutes));
        }

        options.LifetimeInMinutes = jwtOptions.LifetimeInMinutes;

        options.SignatureKey = jwtOptions.SignatureKey;
    });

builder.Services.AddOptions<OpenAIOptions>()
    .Configure(options => {
        var openAIOptions = builder.Configuration.GetSection("OpenAI").Get<OpenAIOptions>();

        if(openAIOptions == null) {
            throw new ArgumentNullException(nameof(openAIOptions));
        }

        options.ApiKey = openAIOptions.ApiKey ?? throw new ArgumentNullException(nameof(openAIOptions.ApiKey));
        options.Model = openAIOptions.Model ?? throw new ArgumentNullException(nameof(openAIOptions.Model));
    });

builder.Services.AddHttpClient();
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.InitAspnetIdentity(builder.Configuration);
builder.Services.InitAuth();
builder.Services.InitSwagger();

builder.Services.AddHangfire(config =>
{
    string conStr = builder.Configuration.GetConnectionString("SqlDb")!;
    config.UsePostgreSqlStorage(conStr);
});
builder.Services.AddHangfireServer();

builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ICVRepository, CVRepository>();
builder.Services.AddScoped<ICVService, CVService>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();
builder.Services.AddScoped<AnalyticsService>();
builder.Services.AddScoped<CVParserService>();
builder.Services.AddScoped<CVsCleanupJob>();
builder.Services.AddScoped<OpenAIAnalysisJob>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("SqlDb") ?? "");

var app = builder.Build();

await app.SeedRolesAsync();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<LoggingMiddleware>();

app.MapControllers().AllowAnonymous();

app.MapHealthChecks("/health");

app.UseHangfireDashboard();

app.UseHttpsRedirection();

RecurringJob.AddOrUpdate<CVsCleanupJob>("daily-cv-cleanup", job => job.Run(), Cron.Daily);

app.Run();
