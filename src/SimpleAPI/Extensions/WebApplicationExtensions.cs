using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace SimpleAPI.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MigrateDbContext<TContext>(this WebApplication webHost) where TContext : DbContext
    {
        using (var scope = webHost.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TContext>>();
            var context = services.GetService<TContext>();

            try
            {
                logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);
                context.Database.Migrate();


                logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
            }
        }

        return webHost;
    }
}