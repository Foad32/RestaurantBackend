using Restaurant.Core.Contracts.Repository;

namespace Restaurant.Core.Contracts.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        //public IBranchRepository Branch { get; }
        public IRegisterUserRepository Register { get; }
        void ClearChangeTracker();

        int Save();
        Task<int> SaveAsync();
    }
}