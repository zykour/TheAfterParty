﻿using System;
using System.Collections.Generic;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;

namespace TheAfterParty.Domain.Abstract
{
    public interface IObjectiveRepository : IDisposable
    {
        AppIdentityDbContext GetContext();

        IEnumerable<Objective> SearchObjectives(string searchText, int resultsLimit);
        IEnumerable<Objective> GetObjectives();
        Objective GetObjectiveByID(int objectiveId);
        void InsertObjective(Objective objective);
        void UpdateObjective(Objective objective);
        void DeleteObjective(int objectiveId);

        IEnumerable<BoostedObjective> GetBoostedObjectives();
        BoostedObjective GetBoostedObjectiveByID(int boostedObjectiveId);
        void InsertBoostedObjective(BoostedObjective boostedObjective);
        void UpdateBoostedObjective(BoostedObjective boostedObjective);
        void DeleteBoostedObjective(int boostedObjectiveId);

        void Save();
    }
}
