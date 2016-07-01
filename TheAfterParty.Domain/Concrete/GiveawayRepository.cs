using System;
using System.Collections.Generic;
using System.Linq;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Concrete
{
    public class GiveawayRepository : IGiveawayRepository, IDisposable
    {
        private AppIdentityDbContext context;
        public AppIdentityDbContext GetContext()
        {
            return context;
        }

        public GiveawayRepository(IUnitOfWork unitOfWork)
        {
            this.context = unitOfWork.DbContext;
        }


        public IEnumerable<Giveaway> GetGiveaways()
        {
            return context.Giveaways.ToList();
        }
        public Giveaway GetGiveawayByID(int giveawayId)
        {
            return context.Giveaways.Find(giveawayId);
        }
        public void InsertGiveaway(Giveaway giveaway)
        {
            context.Giveaways.Add(giveaway);
        }
        public void UpdateGiveaway(Giveaway giveaway)
        {
            Giveaway targetGiveaway = context.Giveaways.Find(giveaway.GiveawayID);

            if (targetGiveaway != null)
            {
                targetGiveaway.EndDate = giveaway.EndDate;
                targetGiveaway.EntryFee = giveaway.EntryFee;
                targetGiveaway.ListingID = giveaway.ListingID;
                targetGiveaway.PointsPrize = giveaway.PointsPrize;
                targetGiveaway.Prize = giveaway.Prize;
                targetGiveaway.CreatedTime = giveaway.CreatedTime;
                targetGiveaway.StartDate = giveaway.StartDate;
                targetGiveaway.CreatorID = giveaway.CreatorID;
                targetGiveaway.WinnerID = giveaway.WinnerID;
            }

            foreach (GiveawayEntry entry in giveaway.GiveawayEntries)
            {
                if (entry.GiveawayEntryID == 0)
                {
                    InsertGiveawayEntry(entry);
                }
                else
                {
                    UpdateGiveawayEntry(entry);
                }
            }
        }
        public void DeleteGiveaway(int giveawayId)
        {
            Giveaway targetGiveaway = context.Giveaways.Find(giveawayId);

            if (targetGiveaway != null)
            {
                context.Giveaways.Remove(targetGiveaway);
            }
        }

        public IEnumerable<GiveawayEntry> GetGiveawayEntries()
        {
            return context.GiveawayEntries.ToList();
        }
        public GiveawayEntry GetGiveawayEntryByID(int giveawayEntryId)
        {
            return context.GiveawayEntries.Find(giveawayEntryId);
        }
        public void InsertGiveawayEntry(GiveawayEntry giveawayEntry)
        {
            context.GiveawayEntries.Add(giveawayEntry);
        }
        public void UpdateGiveawayEntry(GiveawayEntry giveawayEntry)
        {
            GiveawayEntry targetGiveawayEntry = context.GiveawayEntries.Find(giveawayEntry.GiveawayEntryID);

            if (targetGiveawayEntry != null)
            {
                targetGiveawayEntry.EntryDate = giveawayEntry.EntryDate;
                targetGiveawayEntry.HasDonated = giveawayEntry.HasDonated;
                targetGiveawayEntry.UserID = giveawayEntry.UserID;
            }
        }
        public void DeleteGiveawayEntry(int giveawayEntryId)
        {
            GiveawayEntry targetGiveawayEntry = context.GiveawayEntries.Find(giveawayEntryId);

            if (targetGiveawayEntry != null)
            {
                context.GiveawayEntries.Remove(targetGiveawayEntry);
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
