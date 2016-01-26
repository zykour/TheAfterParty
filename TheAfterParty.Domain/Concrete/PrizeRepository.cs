using System;
using System.Collections.Generic;
using System.Linq;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Concrete
{
    public class PrizeRepository : IPrizeRepository, IDisposable
    {
        private AppIdentityDbContext context;
        public AppIdentityDbContext GetContext()
        {
            return context;
        }

        public PrizeRepository(IUnitOfWork unitOfWork)
        {
            this.context = unitOfWork.DbContext;
        }


        public IEnumerable<Prize> GetPrizes()
        {
            return context.Prizes.ToList();
        }
        public Prize GetPrizeByID(int prizeId)
        {
            return context.Prizes.Find(prizeId);
        }
        public void InsertPrize(Prize prize)
        {
            context.Prizes.Add(prize);
        }
        public void UpdatePrize(Prize prize)
        {
            Prize targetPrize = context.Prizes.Find(prize.PrizeID);

            if (targetPrize != null)
            {
                targetPrize.ChancePerThousand = prize.ChancePerThousand;
                targetPrize.Description = prize.Description;
                targetPrize.IsAvailable = prize.IsAvailable;
                targetPrize.Tier = prize.Tier;
            }
            
            foreach (WonPrize entry in prize.WonPrizes)
            {
                if (entry.WonPrizeID == 0)
                {
                    InsertWonPrize(entry);
                }
                else
                {
                    UpdateWonPrize(entry);
                }
            }
        }
        public void DeletePrize(int prizeId)
        {
            Prize targetPrize = context.Prizes.Find(prizeId);

            if (targetPrize != null)
            {
                context.Prizes.Remove(targetPrize);
            }
        }


        public IEnumerable<WonPrize> GetWonPrizes()
        {
            return context.WonPrizes.ToList();
        }
        public WonPrize GetWonPrizeByID(int wonPrizeId)
        {
            return context.WonPrizes.Find(wonPrizeId);
        }
        public void InsertWonPrize(WonPrize wonPrize)
        {
            context.WonPrizes.Add(wonPrize);
        }
        public void UpdateWonPrize(WonPrize wonPrize)
        {
            WonPrize targetWonPrize = context.WonPrizes.Find(wonPrize.WonPrizeID);

            if (targetWonPrize != null)
            {
                targetWonPrize.WinningAction = wonPrize.WinningAction;
            }
        }
        public void DeleteWonPrize(int wonPrizeId)
        {
            WonPrize targetWonPrize = context.WonPrizes.Find(wonPrizeId);

            if (targetWonPrize != null)
            {
                context.WonPrizes.Remove(targetWonPrize);
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
