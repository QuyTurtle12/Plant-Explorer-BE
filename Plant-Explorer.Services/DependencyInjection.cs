using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plant_Explorer.Contract.Repositories.Interface;
using Plant_Explorer.Contract.Services.Interface;
using Plant_Explorer.Repositories.Repositories;
using Plant_Explorer.Services.Services;

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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IClaimsTransformation, CustomClaimsTransformer>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IAvatarService, AvatarService>();
            services.AddScoped<IPlantImageService, PlantImageService>();

        }
    }
}
