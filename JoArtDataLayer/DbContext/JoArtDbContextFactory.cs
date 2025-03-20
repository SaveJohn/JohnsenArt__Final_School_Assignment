using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace JoArtDataLayer;

public class JoArtDbContextFactory : IDesignTimeDbContextFactory<JoArtDbContext>
{
    public JoArtDbContext CreateDbContext(string[] args)
        // This class creates an instance of JoArtDbContext which is necessary in a class library without a program.cs
    {
        var jsonConfiguration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.DesignTime.json", optional: false, reloadOnChange: true)
            .Build();


        string connectionString = jsonConfiguration.GetConnectionString("DefaultConnection")
                                  ?? throw new InvalidOperationException("Connection string was not found.");

        var optionsBuilder = new DbContextOptionsBuilder<JoArtDbContext>();
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));


        return new JoArtDbContext(optionsBuilder.Options);
    }
}