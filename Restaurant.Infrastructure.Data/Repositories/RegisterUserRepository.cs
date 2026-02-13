using Microsoft.EntityFrameworkCore;
using Restaurant.Core.Contracts.Repository;
using Restaurant.Core.Domain.Models;
using Restaurant.Infrastructure.Data.Common;
using Restaurant.Infrastructure.EF;

namespace Restaurant.Infrastructure.Data.Repositories;

public sealed class RegisterUserRepository(DemoContext context)
    : GenericRepository<User>(context), IRegisterUserRepository
{
    public async Task<User?> GetUserByPhoneNumber(string phoneNumber)
    {
        return await Context.Users.SingleOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
    }

    public async Task<long> GenerateUserId()
    {
        var HasUser = await Context.Users.AnyAsync();
        return HasUser ? Context.Users.Max(x => x.UserId) + 1 : 1;
    }
}
