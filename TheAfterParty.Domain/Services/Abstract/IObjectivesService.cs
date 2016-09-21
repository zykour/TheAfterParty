using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Services
{
    public interface IObjectivesService
    {
        void SetUserName(string userName);
        Task<AppUser> GetCurrentUser();
        AppUser GetCurrentUserSynch();

        Tag GetTagByID(int id);
        IEnumerable<Objective> GetObjectives();
        Objective GetObjectiveByID(int id);
        void AddObjective(Objective objective);
        void EditObjective(Objective objective, int productId);
        IEnumerable<BoostedObjective> GetBoostedObjectives();
        BoostedObjective GetBoostedObjectiveByID(int id);
        void AddBoostedObjective(BoostedObjective boostedObjective);
        void EditBoostedObjective(BoostedObjective boostedObjective, int days);
        void DeleteBoostedObjective(int id);
        Product GetProductByID(int id);
        IEnumerable<Tag> GetTags();
    }
}
