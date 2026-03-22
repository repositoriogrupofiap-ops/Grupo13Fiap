using Microsoft.Extensions.Configuration;

namespace Grupo13Fiap.Utils.Extensions;

public static class ConfigurationExtensions
{
    public static string GetConnectionStringDataBase(this IConfiguration configuration)
    {
        var host = Environment.GetEnvironmentVariable("SQLSERVER_HOST");
        var port = Environment.GetEnvironmentVariable("SQLSERVER_PORT");
        var database = Environment.GetEnvironmentVariable("SQLSERVER_DATABASE");
        var user = Environment.GetEnvironmentVariable("SQLSERVER_USER");
        var password = Environment.GetEnvironmentVariable("SQLSERVER_PASSWORD");

        if(string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            return configuration.GetConnectionString("DefaultConnection")!;

        return string.Format(
            configuration.GetConnectionString("DefaultConnection")!,
            host, port, database, user, password);
    }
}
