using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Core.Contracts.Repository;
using Restaurant.Core.Contracts.UnitOfWork;
using Restaurant.Infrastructure.EF;
using Restaurant.Infrastructure.Data.Repositories;

namespace Restaurant.Core.ApplicationService.UnitOfWork
{
    internal sealed class UnitofWork1 : IUnitOfWork
    {
        private DemoContext context;
        //private EnLogsContext LogsContext;
        private IConfiguration configure;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IServiceScopeFactory serviceScopeFactory;

        private readonly IHttpClientFactory httpClientFactory;


        public UnitofWork1(DemoContext context, IHttpContextAccessor _httpContextAccessor, IConfiguration configure,
        IServiceScopeFactory serviceScopeFactory, IHttpClientFactory httpClientFactory)
        {
            this.context = context;
            this._httpContextAccessor = _httpContextAccessor;
            this.configure = configure;
            this.serviceScopeFactory = serviceScopeFactory;
            this.httpClientFactory = httpClientFactory;


            //Branch = new BranchRepository(this.context);
            Register = new RegisterUserRepository(this.context);
          
        }

        //public IBranchRepository Branch { get; private set; }
        public IRegisterUserRepository Register { get; private set; }
       

        public void Dispose()
        {
            context.Dispose();
        }
        public int Save()
        {
            return context.SaveChanges();
        }
        public Task<int> SaveAsync()
        {
            return context.SaveChangesAsync();
        }
        public void ClearChangeTracker()
        {
            context.ChangeTracker.Clear();
        }
    }
}
