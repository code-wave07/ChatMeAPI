using ChatMe.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ChatMe.API
{
    public class ChatMeContextFactory : IDesignTimeDbContextFactory<ChatMeDbContext>
    {
        public ChatMeDbContext CreateDbContext(string[] args)
        {
            // 1. Build Configuration to read appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // 2. Get Connection String
            var builder = new DbContextOptionsBuilder<ChatMeDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString, b => b.MigrationsAssembly("ChatMe.Data"));

            return new ChatMeDbContext(builder.Options);
        }
    }
}