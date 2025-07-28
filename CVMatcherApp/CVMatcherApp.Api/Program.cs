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

builder.Services.AddScoped<IAuditLogger, AuditLogger>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddScoped<ICVRepository, CVRepository>();
builder.Services.AddScoped<ICVService, CVService>();
builder.Services.AddScoped<CVsCleanupJob>();

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

var app = builder.Build();

await app.SeedRolesAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LoggingMiddleware>();

app.MapControllers();

app.UseHangfireDashboard();

app.UseHttpsRedirection();

RecurringJob.AddOrUpdate<CVsCleanupJob>("daily-cv-cleanup", job => job.Run(), Cron.Daily);

app.Run();
