using System;
using System.Collections.Generic;
using System.Linq;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Concrete
{
    public class CouponRepository : ICouponRepository, IDisposable
    {
        private AppIdentityDbContext context;
        public AppIdentityDbContext GetContext()
        {
            return context;
        }

        public CouponRepository(IUnitOfWork unitOfWork)
        {
            this.context = unitOfWork.DbContext;
        }


        public IEnumerable<Coupon> GetCoupons()
        {
            return context.Coupons.ToList();
        }
        public Coupon GetCouponByID(int couponId)
        {
            return context.Coupons.Find(couponId);
        }
        public void InsertCoupon(Coupon coupon)
        {
            context.Coupons.Add(coupon);

            foreach (UserCoupon userCoupon in coupon.UserCoupons)
            {
                if (userCoupon.UserCouponID == 0)
                {
                    InsertUserCoupon(userCoupon);
                }
                else
                {
                    UpdateUserCoupon(userCoupon);
                }
            }
        }
        public void UpdateCoupon(Coupon coupon)
        {
            Coupon targetCoupon = context.Coupons.Find(coupon.CouponID);

            if (targetCoupon != null)
            {
                targetCoupon.CouponName = coupon.CouponName;
                targetCoupon.DiscountPercent = coupon.DiscountPercent;
                targetCoupon.Expiry = coupon.Expiry;
                targetCoupon.IsOrderWide = coupon.IsOrderWide;
                targetCoupon.IsStackable = coupon.IsStackable;
                targetCoupon.ListingID = coupon.ListingID;
            }

            foreach (UserCoupon userCoupon in coupon.UserCoupons)
            {
                if (userCoupon.UserCouponID == 0)
                {
                    InsertUserCoupon(userCoupon);
                }
                else
                {
                    UpdateUserCoupon(userCoupon);
                }
            }
        }
        public void DeleteCoupon(int couponId)
        {
            Coupon targetCoupon = context.Coupons.Find(couponId);

            if (targetCoupon != null)
            {
                context.Coupons.Remove(targetCoupon);
            }
        }

        public IEnumerable<UserCoupon> GetUserCoupons()
        {
            return context.UserCoupons.ToList();
        }
        public UserCoupon GetUserCouponByID(int userCouponId)
        {
            return context.UserCoupons.Find(userCouponId);
        }
        public void InsertUserCoupon(UserCoupon userCoupon)
        {
            context.UserCoupons.Add(userCoupon);
        }
        public void UpdateUserCoupon(UserCoupon userCoupon)
        {
            UserCoupon targetUserCoupon = context.UserCoupons.Find(userCoupon.UserCouponID);

            if (targetUserCoupon != null)
            {
                targetUserCoupon.Quantity = userCoupon.Quantity;
            }
        }
        public void DeleteUserCoupon(int userCouponId)
        {
            UserCoupon targetUserCoupon = context.UserCoupons.Find(userCouponId);

            if (targetUserCoupon != null)
            {
                context.UserCoupons.Remove(targetUserCoupon);
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
