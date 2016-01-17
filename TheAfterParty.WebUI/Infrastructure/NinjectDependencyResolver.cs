using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Concrete;
using TheAfterParty.Domain.Services;
using Ninject;
using Ninject.Web.Common;

namespace TheAfterParty.WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernel.Bind<IRepository>().To<EFRepository>();
            kernel.Bind<AppIdentityDbContext>().ToSelf().InRequestScope();

            kernel.Bind<IListingRepository>().To<ListingRepository>().InRequestScope();
            kernel.Bind<IUserRepository>().To<UserRepository>().InRequestScope();

            kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();

            kernel.Bind<ICartService>().To<CartService>().InRequestScope();
        }
    }
}