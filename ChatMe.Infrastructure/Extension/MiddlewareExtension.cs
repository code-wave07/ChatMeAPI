using ChatMe.Data.Context;
using ChatMe.Data.Implementations;
using ChatMe.Data.Interfaces;
using ChatMe.Infrastructure.Implementations;
using ChatMe.Infrastructure.Interfaces;
using ChatMe.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatMe.Infrastructure.Extensions
{
    public static class MiddlewareExtension
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. DATABASE SETUP
            services.AddDbContext<ChatMeDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            // 2. IDENTITY SETUP
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ChatMeDbContext>()
                .AddDefaultTokenProviders();

            // 3. REPOSITORIES & UNIT OF WORK (Use Scoped for DB related stuff)
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

            // 4. SIGNALR (Requires the .csproj fix above)
            services.AddSignalR();

            // 5. DOMAIN SERVICES (We will create these next)
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            // services.AddTransient<IChatService, ChatService>();
        }
    }
}