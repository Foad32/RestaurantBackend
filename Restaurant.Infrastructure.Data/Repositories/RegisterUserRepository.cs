using Restaurant.Core.Contracts.Repository;
using Restaurant.Core.Domain.Models;
using Restaurant.Infrastructure.Data.Common;
using Restaurant.Infrastructure.EF;

namespace Restaurant.Infrastructure.Data.Repositories;

public sealed class RegisterUserRepository(DemoContext context)
    : GenericRepository<User>(context), IRegisterUserRepository
{

}
