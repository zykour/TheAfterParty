[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(TheAfterParty.WebUI.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(TheAfterParty.WebUI.App_Start.NinjectWebCommon), "Stop")]

namespace TheAfterParty.WebUI.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    using Domain.Services;
    using Domain.Abstract;
    using Domain.Concrete;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        public static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<AppIdentityDbContext>().ToSelf().InRequestScope();

            kernel.Bind<IAuctionRepository>().To<AuctionRepository>().InRequestScope();
            kernel.Bind<ICouponRepository>().To<CouponRepository>().InRequestScope();
            kernel.Bind<IGiveawayRepository>().To<GiveawayRepository>().InRequestScope();
            kernel.Bind<IListingRepository>().To<ListingRepository>().InRequestScope();
            kernel.Bind<IObjectiveRepository>().To<ObjectiveRepository>().InRequestScope();
            kernel.Bind<IPrizeRepository>().To<PrizeRepository>().InRequestScope();
            kernel.Bind<IUserRepository>().To<UserRepository>().InRequestScope();
            kernel.Bind<ISiteRepository>().To<SiteRepository>().InRequestScope();

            kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();

            kernel.Bind<ICartService>().To<CartService>().InRequestScope();
            kernel.Bind<IStoreService>().To<StoreService>().InRequestScope();
            kernel.Bind<IUserService>().To<UserService>().InRequestScope();
            kernel.Bind<ISiteService>().To<SiteService>().InRequestScope();
            kernel.Bind<IObjectivesService>().To<ObjectivesService>().InRequestScope();
            kernel.Bind<IAuctionService>().To<AuctionService>().InRequestScope();
            kernel.Bind<IApiService>().To<ApiService>().InRequestScope();
            kernel.Bind<IMemoryCache>().To<MemoryCache>().InSingletonScope().WithConstructorArgument<MemoryCacheOptions>(new MemoryCacheOptions());
            kernel.Bind<IOptions<MemoryCacheOptions>>().To<MemoryCacheOptions>().InSingletonScope();
        }        
    }
}
