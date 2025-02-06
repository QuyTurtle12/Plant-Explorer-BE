using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Repositories.Repositories;

namespace Plant_Explorer.Services
{
    public static class DependencyInjection
    {

        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepository();
            services.AddAutoMapper();
            services.AddServices(configuration);
        }

        public static void AddRepository(this IServiceCollection services)
        {
            services
                .AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        }

        private static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddScoped<ITokenService, TokenService>();
            //services.AddScoped<IAuthService, AuthService>();
            

        }
    }
}
