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

            // 3. REPOSITORIES
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // --- FIX IS HERE ---
            // We register the non-generic IUnitOfWork to the specific ChatMeDbContext implementation
            services.AddScoped<IUnitOfWork, UnitOfWork<ChatMeDbContext>>();
            // -------------------

            // 4. SIGNALR
            services.AddSignalR();

            // 5. SERVICES
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IChatService, ChatService>(); // ChatService needs Scoped because UnitOfWork is Scoped
        }
    }
}