using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Services;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace TheAfterParty.Domain.Services
{
    public class ObjectivesService : IObjectivesService
    {
        private IObjectiveRepository objectiveRepository;
        private IListingRepository listingRepository;
        private IUnitOfWork unitOfWork;
        public AppUserManager UserManager { get; private set; }
        public string userName { get; set; }

        public ObjectivesService(IObjectiveRepository objectiveRepository, IListingRepository listingRepository, IUnitOfWork unitOfWork) : this(new AppUserManager(new UserStore<AppUser>(unitOfWork.DbContext)))
        {
            this.objectiveRepository = objectiveRepository;
            this.listingRepository = listingRepository;
            this.unitOfWork = unitOfWork;
        }
        protected ObjectivesService(AppUserManager userManager)
        {
            UserManager = userManager;
        }

        public void SetUserName(string userName)
        {
            this.userName = userName;
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectiveRepository.GetObjectives();
        }

        public Objective GetObjectiveByID(int id)
        {
            return objectiveRepository.GetObjectiveByID(id);
        }

        public void AddObjective(Objective objective)
        {
            objectiveRepository.InsertObjective(objective);
            unitOfWork.Save();
        }

        public void EditObjective(Objective objective, int productId)
        {
            Objective updatedObjective = objectiveRepository.GetObjectiveByID(objective.ObjectiveID);

            updatedObjective.Category = objective.Category;
            updatedObjective.Description = objective.Description;
            updatedObjective.IsActive = objective.IsActive;
            updatedObjective.ObjectiveName = objective.ObjectiveName;
            updatedObjective.RequiresAdmin = objective.RequiresAdmin;
            updatedObjective.Reward = objective.Reward;
            updatedObjective.Title = objective.Title;

            if (productId != 0 && updatedObjective.Product != null && updatedObjective.Product.ProductID != productId)
            {
                updatedObjective.Product = listingRepository.GetProductByID(productId);
            }

            objectiveRepository.UpdateObjective(updatedObjective);
            unitOfWork.Save();
        }
        
        public IEnumerable<BoostedObjective> GetBoostedObjectives()
        {
            return objectiveRepository.GetBoostedObjectives();
        }

        public BoostedObjective GetBoostedObjectiveByID(int id)
        {
            return objectiveRepository.GetBoostedObjectiveByID(id);
        }

        public void AddBoostedObjective(BoostedObjective boostedObjective)
        {
            objectiveRepository.InsertBoostedObjective(boostedObjective);
            unitOfWork.Save();
        }

        public void EditBoostedObjective(BoostedObjective boostedObjective, int days)
        {
            BoostedObjective updatedBoostedObjective = objectiveRepository.GetBoostedObjectiveByID(boostedObjective.BoostedObjectiveID);

            updatedBoostedObjective.BoostAmount = boostedObjective.BoostAmount;
            updatedBoostedObjective.EndDate = updatedBoostedObjective.EndDate.AddDays(days);

            objectiveRepository.UpdateBoostedObjective(updatedBoostedObjective);
            unitOfWork.Save();
        }

        public Product GetProductByID(int id)
        {
            return listingRepository.GetProductByID(id);
        }

        public IEnumerable<Tag> GetTags()
        {
            return listingRepository.GetTags();
        }

        public Tag GetTagByID(int id)
        {
            return listingRepository.GetTagByID(id);
        }

        public void Dispose()
        {
            this.unitOfWork.Dispose();
        }

        public AppUser GetCurrentUserSynch()
        {
            return UserManager.FindByName(userName);
        }

        public async Task<AppUser> GetCurrentUser()
        {
            return await UserManager.FindByNameAsync(userName);
        }
    }
}
