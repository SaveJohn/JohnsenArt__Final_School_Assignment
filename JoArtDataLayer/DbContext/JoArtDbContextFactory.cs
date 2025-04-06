using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace JoArtDataLayer;

public class JoArtDbContextFactory : IDesignTimeDbContextFactory<JoArtDbContext>
{
    public JoArtDbContext CreateDbContext(string[] args)
    {
       
        var basePath = AppContext.BaseDirectory;

        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.DesignTime.json", optional: false)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string not found");

        var optionsBuilder = new DbContextOptionsBuilder<JoArtDbContext>();
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));

        return new JoArtDbContext(optionsBuilder.Options);
    }
}