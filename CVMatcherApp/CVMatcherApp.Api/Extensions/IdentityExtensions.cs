using CVMatcherApp.Api.Data;
using CVMatcherApp.Api.Models;
using CVMatcherApp.Api.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CVMatcherApp.Api.Extensions;
public static class IdentityExtensions
{
    public static void InitAspnetIdentity(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddDbContext<MatcherDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("SqlDb");
            options.UseNpgsql(connectionString);
        });

        serviceCollection.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<MatcherDbContext>();
    }

    public async static Task SeedRolesAsync(this WebApplication app) {
        var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await roleManager.CreateAsync(new IdentityRole("User"));
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    public static void InitAuth(this IServiceCollection services) {
        var jwtOptions = services.BuildServiceProvider().GetRequiredService<IOptions<JwtOptions>>().Value;
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                var keyStr = jwtOptions.SignatureKey;
                System.Console.WriteLine(keyStr);
                var keyBytes = System.Text.Encoding.ASCII.GetBytes(keyStr);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
                };
            });
    }
}