using System;
using System.Collections.Generic;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;

namespace TheAfterParty.Domain.Abstract
{
    public interface IGiveawayRepository : IDisposable
    {
        AppIdentityDbContext GetContext();

        IEnumerable<Giveaway> GetGiveaways();
        Giveaway GetGiveawayByID(int giveawayId);
        void InsertGiveaway(Giveaway giveaway);
        void UpdateGiveaway(Giveaway giveaway);
        void DeleteGiveaway(int giveawayId);

        IEnumerable<GiveawayEntry> GetGiveawayEntries();
        GiveawayEntry GetGiveawayEntryByID(int giveawayEntryId);
        void InsertGiveawayEntry(GiveawayEntry giveawayEntry);
        void UpdateGiveawayEntry(GiveawayEntry giveawayEntry);
        void DeleteGiveawayEntry(int giveawayEntryId);

        void Save();
    }
}
