using System;
using System.Collections.Generic;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;

namespace TheAfterParty.Domain.Abstract
{
    public interface IPrizeRepository : IDisposable
    {
        AppIdentityDbContext GetContext();

        IEnumerable<Prize> GetPrizes();
        Prize GetAuctionByID(int prizeId);
        void InsertPrize(Prize prize);
        void UpdatePrize(Prize prize);
        void DeletePrize(int prizeId);

        IEnumerable<WonPrize> GetWonPrizes();
        WonPrize GetWonPrizeByID(int wonPrizeId);
        void InsertWonPrize(WonPrize wonPrize);
        void UpdateWonPrize(WonPrize wonPrize);
        void DeleteWonPrize(int wonPrizeId);

        void Save();
    }
}
