﻿using System;
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

            if (objective.BoostedObjective != null)
            {
                if (objective.BoostedObjective.BoostedObjectiveID == 0)
                {
                    InsertBoostedObjective(objective.BoostedObjective);
                }
            }
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
                targetObjective.IsActive = objective.IsActive;
                targetObjective.Title = objective.Title;
                targetObjective.ObjectiveName = objective.ObjectiveName;
            }

            if (objective.BoostedObjective != null)
            {
                if (objective.BoostedObjective.BoostedObjectiveID == 0)
                {
                    InsertBoostedObjective(objective.BoostedObjective);
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
            BoostedObjective targetBoostedObjective = context.BoostedObjectives.Find(boostedObjective.BoostedObjectiveID);
             
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
