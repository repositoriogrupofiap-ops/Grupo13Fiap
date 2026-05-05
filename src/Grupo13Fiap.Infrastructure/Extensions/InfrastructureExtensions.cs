using Grupo13Fiap.Application.Interfaces.Services;
using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.Identity.Configurations;
using Grupo13Fiap.Identity.Data;
using Grupo13Fiap.Identity.Services;
using Grupo13Fiap.Infrastructure.Context;
using Grupo13Fiap.Infrastructure.Data;
using Grupo13Fiap.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Scalar.AspNetCore;

namespace Grupo13Fiap.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder>? configureDb = null)
    {
        services.AddDbContext(configuration, configureDb);
        services.AddRepositories();
        services.AddAuthentication(configuration);
        services.AddAuthorizationPolicies();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider provider)
    {
        using var scope = provider.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<DBContextGrupo13Fiap>();
        if (!db.Database.IsInMemory())
        {
            await db.Database.MigrateAsync();
        }
        else
        {
            await db.Database.EnsureCreatedAsync();
        }

        var identityDb = scope.ServiceProvider.GetRequiredService<IdentityDataContext>();
        if (!identityDb.Database.IsInMemory())
        {
            await identityDb.Database.MigrateAsync();
        }
        else
        {
            await identityDb.Database.EnsureCreatedAsync();
        }

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        await IdentityDataSeeder.SeedAsync(roleManager, userManager);
    }

    public static async Task SeedDatabaseAsync(this IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DBContextGrupo13Fiap>();

        await DataSeeder.SeedAsync(db);
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder>? configureDb = null)
    {
        if (configureDb is not null)
        {
            services.AddDbContext<DBContextGrupo13Fiap>(configureDb);
            services.AddDbContext<IdentityDataContext>(configureDb);
            return services;
        }

        services.AddDbContext<DBContextGrupo13Fiap>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddDbContext<IdentityDataContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>()
                .AddDefaultTokenProviders();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ILibraryRepository, LibraryRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IGameRepository, GameRepository>();

        return services;
    }

    private static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtAppSettingOptions = configuration.GetSection(nameof(JwtOptions));
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JwtOptions:SecurityKey").Value));

        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = jwtAppSettingOptions[nameof(JwtOptions.Issuer)];
            options.Audience = jwtAppSettingOptions[nameof(JwtOptions.Audience)];
            options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
            options.AccessTokenExpiration = int.Parse(jwtAppSettingOptions[nameof(JwtOptions.AccessTokenExpiration)] ?? "0");
            options.RefreshTokenExpiration = int.Parse(jwtAppSettingOptions[nameof(JwtOptions.RefreshTokenExpiration)] ?? "0");
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
        });

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = configuration.GetSection("JwtOptions:Issuer").Value,

            ValidateAudience = true,
            ValidAudience = configuration.GetSection("JwtOptions:Audience").Value,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,

            RequireExpirationTime = true,
            ValidateLifetime = true,

            RoleClaimType = "role",

            ClockSkew = TimeSpan.Zero
        };

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = tokenValidationParameters;
        });
    }

    private static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization();
    }

}
