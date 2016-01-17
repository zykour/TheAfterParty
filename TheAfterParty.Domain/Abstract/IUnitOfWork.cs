using System;
using TheAfterParty.Domain.Concrete;

namespace TheAfterParty.Domain.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        AppIdentityDbContext DbContext { get; }
        int Save();
    }
}
    