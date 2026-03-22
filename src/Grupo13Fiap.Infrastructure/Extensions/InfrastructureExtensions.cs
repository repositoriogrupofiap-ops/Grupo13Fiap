using Grupo13Fiap.Domain.Interfaces;
using Grupo13Fiap.Infrastructure.Context;
using Grupo13Fiap.Infrastructure.Data;
using Grupo13Fiap.Infrastructure.Repositories;
using Grupo13Fiap.Utils.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grupo13Fiap.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext(configuration);
        services.AddRepositories();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DBContextGrupo13Fiap>();

        await db.Database.EnsureCreatedAsync();   // InMemory  — usar EnsureCreatedAsync
        // await db.Database.MigrateAsync();       // SQL Server — trocar para MigrateAsync

        await DataSeeder.SeedAsync(db);
    }

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // Método 1 — InMemory (ativo para testes locais)
        services.AddDbContext<DBContextGrupo13Fiap>(options =>
            options.UseInMemoryDatabase("Grupo13FiapDb"));

        // Método 2 — SQL Server via variável de ambiente (descomentar para produção)
        // services.AddDbContext<DBContextGrupo13Fiap>(options =>
        //     options.UseSqlServer(configuration.GetConnectionStringDataBase()));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUsersRepository,   UsersRepository>();
        services.AddScoped<ILibraryRepository, LibraryRepository>();
        services.AddScoped<IStoreRepository,   StoreRepository>();
        services.AddScoped<IGameRepository,    GameRespository>();

        return services;
    }
}
