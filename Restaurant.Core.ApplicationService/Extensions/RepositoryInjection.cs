using Microsoft.Extensions.DependencyInjection;
using Restaurant.Core.ApplicationService.UnitOfWork;
using Restaurant.Core.Contracts.UnitOfWork;

namespace Restaurant.Core.ApplicationService.Extensions
{
    public static class RepositoryInjection
    {
        public static void AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitofWork1>();
        } 
    }
}
