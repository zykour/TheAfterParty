using System;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Abstract;

namespace TheAfterParty.Domain.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppIdentityDbContext context = new AppIdentityDbContext();
        public AppIdentityDbContext DbContext
        {
            get
            {
                if (context == null)
                {
                    context = new AppIdentityDbContext();
                }

                return context;
            }
        }

        public UnitOfWork() { }
        public UnitOfWork(AppIdentityDbContext context)
        {
            this.context = context;
        }

        public int Save()
        {
            return context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}