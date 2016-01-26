using System;
using System.Collections.Generic;
using System.Linq;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Concrete
{
    public class ObjectiveRepository : IObjectiveRepository, IDisposable
    {
        private AppIdentityDbContext context;
        public AppIdentityDbContext GetContext()
        {
            return context;
        }

        public ObjectiveRepository(IUnitOfWork unitOfWork)
        {
            this.context = unitOfWork.DbContext;
        }



        public IEnumerable<Objective> GetObjectives()
        {
            return context.Objectives.ToList();
        }
        public Objective GetObjectiveByID(int objectiveId)
        {
            return context.Objectives.Find(objectiveId);
        }
        public void InsertObjective(Objective objective)
        {
            context.Objectives.Add(objective);
        }
        public void UpdateObjective(Objective objective)
        {
            Objective targetObjective = context.Objectives.Find(objective.ObjectiveID);

            if (targetObjective != null)
            {
                targetObjective.Description = objective.Description;
                targetObjective.Category = objective.Category;
                targetObjective.RequiresAdmin = objective.RequiresAdmin;
                targetObjective.Reward = objective.Reward;
            }

            if (objective.BoostedObjective != null && objective.BoostedObjective.ObjectiveID == 0)
            {
                InsertBoostedObjective(objective.BoostedObjective);
            }

            foreach (ObjectiveGameMapping mapping in objective.ObjectiveGameMappings)
            {
                if (mapping.Id == 0)
                {
                    InsertObjectiveGameMapping(mapping);
                }
            }
        }
        public void DeleteObjective(int objectiveId)
        {
            Objective targetObjective = context.Objectives.Find(objectiveId);

            if (targetObjective != null)
            {
                context.Objectives.Remove(targetObjective);
            }
        }

        public IEnumerable<BoostedObjective> GetBoostedObjectives()
        {
            return context.BoostedObjectives.ToList();
        }
        public BoostedObjective GetBoostedObjectiveByID(int boostedObjectiveId)
        {
            return context.BoostedObjectives.Find(boostedObjectiveId);
        }
        public void InsertBoostedObjective(BoostedObjective boostedObjective)
        {
            context.BoostedObjectives.Add(boostedObjective);
        }
        public void UpdateBoostedObjective(BoostedObjective boostedObjective)
        {
            BoostedObjective targetBoostedObjective = context.BoostedObjectives.Find(boostedObjective.ID);
             
            if (targetBoostedObjective != null)
            {
                targetBoostedObjective.BoostAmount = boostedObjective.BoostAmount;
                targetBoostedObjective.EndDate = boostedObjective.EndDate;
            }
        }
        public void DeleteBoostedObjective(int boostedObjectiveId)
        {
            BoostedObjective targetBoostedObjective = context.BoostedObjectives.Find(boostedObjectiveId);

            if (targetBoostedObjective != null)
            {
                context.BoostedObjectives.Remove(targetBoostedObjective);
            }
        }

        public IEnumerable<ObjectiveGameMapping> GetObjectiveGameMappings()
        {
            return context.ObjectiveGameMappings.ToList();
        }
        public ObjectiveGameMapping GetObjectiveGameMappingByID(int objecctiveGameMappingId)
        {
            return context.ObjectiveGameMappings.Find(objecctiveGameMappingId);
        }
        public void InsertObjectiveGameMapping(ObjectiveGameMapping objectiveGameMapping)
        {
            context.ObjectiveGameMappings.Add(objectiveGameMapping);
        }
        public void DeleteObjectiveGameMapping(int objectiveGameMappingId)
        {
            ObjectiveGameMapping targetObjectiveGameMapping = context.ObjectiveGameMappings.Find(objectiveGameMappingId);

            if (targetObjectiveGameMapping != null)
            {
                context.ObjectiveGameMappings.Remove(targetObjectiveGameMapping);
            }
        }

        // ---- Repository methods

        public void Save()
        {
            context.SaveChanges();
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
