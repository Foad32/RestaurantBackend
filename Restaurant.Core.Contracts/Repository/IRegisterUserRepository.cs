using Restaurant.Core.Contracts.Repository.Common;
using Restaurant.Core.Domain.Models;

namespace Restaurant.Core.Contracts.Repository
{
    public interface IRegisterUserRepository : IGenericRepository<User>
    {
        Task<long> GenerateUserId();
        Task<User?> GetUserByPhoneNumber(string phoneNumber);
    }
}
