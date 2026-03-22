using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.Infrastructure.Context;
using Grupo13Fiap.Infrastructure.Data;
using Grupo13Fiap.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grupo13Fiap.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Action<DbContextOptionsBuilder>? configureDb = null)
    {
        services.AddDbContext(configuration, configureDb);
        services.AddRepositories();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DBContextGrupo13Fiap>();

        await db.Database.EnsureCreatedAsync();   // InMemory  — usar EnsureCreatedAsync
        // await db.Database.MigrateAsync();       // SQL Server — trocar para MigrateAsync
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
            return services;
        }

        //coloquei em memoria pra não termos problemas com a conn de inicio, qualquer coisa só descomentar o codigo a baixo e comentar este.
        //é preciso ajustar o metodo a cima InitializeDatabaseAsync para usar migrate ao invés de ensurecreated
        services.AddDbContext<DBContextGrupo13Fiap>(options =>
            options.UseInMemoryDatabase("Grupo13FiapDb"));


        // services.AddDbContext<DBContextGrupo13Fiap>(options =>
        //     options.UseSqlServer(configuration.GetConnectionStringDataBase()));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ILibraryRepository, LibraryRepository>();
        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IGameRepository, GameRepository>();

        return services;
    }
}
