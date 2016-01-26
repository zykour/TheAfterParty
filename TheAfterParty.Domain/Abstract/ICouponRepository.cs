using System;
using System.Collections.Generic;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;

namespace TheAfterParty.Domain.Abstract
{
    public interface ICouponRepository : IDisposable
    {
        AppIdentityDbContext GetContext();
        
        IEnumerable<Coupon> GetCoupons();
        Coupon GetCouponByID(int couponId);
        void InsertCoupon(Coupon coupon);
        void UpdateCoupon(Coupon coupon);
        void DeleteCoupon(int couponId);

        IEnumerable<UserCoupon> GetUserCoupons();
        UserCoupon GetUserCouponByID(int userCouponId);
        void InsertUserCoupon(UserCoupon userCoupon);
        void UpdateUserCoupon(UserCoupon userCoupon);
        void DeleteUserCoupon(int userCouponId);
        
        void Save();
    }
}
