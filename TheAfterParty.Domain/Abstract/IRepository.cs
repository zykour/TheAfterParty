using System.Collections.Generic;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Concrete;

namespace TheAfterParty.Domain.Abstract
{
    public interface IRepository
    {

        AppIdentityDbContext GetContext();
        //AppUser*
        //Auction

        IEnumerable<Auction> Auctions { get; }
        void SaveAuction(Auction auction);

        //AuctionBids

        IEnumerable<AuctionBid> AuctionBids { get; }
        void SaveAuctionBid(AuctionBid auctionBid);
        AuctionBid DeleteAuctionBid(int auctionID, int bidNumber);

        ////BalanceEntry

        IEnumerable<BalanceEntry> BalanceEntries { get; }
        void SaveBalanceEntry(BalanceEntry balanceEntry);

        //BoostedObjective

        IEnumerable<BoostedObjective> BoostedObjectives { get; }
        void SaveBoostedObjective(BoostedObjective boostedObjective);
        BoostedObjective DeleteBoostedObjective(int objectiveID);

        //ClaimedProductKey

        IEnumerable<ClaimedProductKey> ClaimedProductKeys { get; }
        void SaveClaimedProductKey(ClaimedProductKey claimedProductKey);
        ClaimedProductKey DeleteClaimedProductKey(int keyID);

        //Coupon

        IEnumerable<Coupon> Coupons { get; }
        void SaveCoupon(Coupon coupon);

        //DiscountedListing

        IEnumerable<DiscountedListing> DiscountedListings { get; }
        void SaveDiscountedListing(DiscountedListing DiscountedListing);
        DiscountedListing DeleteDiscountedListing(int DiscountedListingID);

        //Gift

        IEnumerable<Gift> Gifts { get; }
        void SaveGift(Gift gift);
        Gift DeleteGift(int GiftID);

        //Giveaway

        IEnumerable<Giveaway> Giveaways { get; }
        void SaveGiveaway(Giveaway giveaway);
        Giveaway DeleteGiveaway(int GiveawayID);

        //GiveawayEntry

        IEnumerable<GiveawayEntry> GiveawayEntries { get; }
        void SaveGiveawayEntry(GiveawayEntry giveawayEntry);
        GiveawayEntry DeleteGiveawayEntry(int givewayID, int UserID);

        //Listing

        IEnumerable<Listing> Listings { get; }
        void SaveListing(Listing listing);
        Listing GetListingById(int listingId);
        //Listing DeleteListing(int listingID);

        //Mail

        IEnumerable<Mail> Mail { get; }
        void SaveMail(Mail mail);
        Mail DeleteMail(int mailID);

        //MappedListing

        IEnumerable<MappedListing> MappedListings { get; }
        void SaveMappedListing(MappedListing mappedListing);
        //MappedListing DeleteMappedListing(int mappedListingID);

        //Objective

        IEnumerable<Objective> Objectives { get; }
        void SaveObjective(Objective objective);
        Objective DeleteObjective(int objectiveID);

        //ObjectiveGameMapping

        IEnumerable<ObjectiveGameMapping> ObjectiveGameMappings { get; }
        void SaveObjectiveGameMapping(ObjectiveGameMapping objectiveGameMapping);
        ObjectiveGameMapping DeleteObjectiveGameMapping(int id);

        //Order

        IEnumerable<Order> Orders { get; }
        void SaveOrder(Order order);
        Order DeleteOrder(int orderID);

        //OwnedGame

        IEnumerable<OwnedGame> OwnedGames { get; }
        void SaveOwnedGame(OwnedGame ownedGame);
        //OwnedGame DeleteOwnedGame(int UserID, int appID);

        //Prize

        IEnumerable<Prize> Prizes { get; }
        void SavePrize(Prize prize);
        //Prize DeletePrize(int prizeID);

        //product

        IEnumerable<Product> Products { get; }
        void SaveProduct(Product product);

        //ListingComment

        IEnumerable<ListingComment> ListingComments { get; }
        void SaveListingComment(ListingComment ListingComment);
        ListingComment DeleteListingComment(int ListingCommentID);

        //ProductDetail

        IEnumerable<ProductDetail> ProductDetails { get; }
        void SaveProductDetail(ProductDetail productDetail);
        ProductDetail DeleteProductDetail(int productId);

        //ProductKey

        IEnumerable<ProductKey> ProductKeys { get; }
        void SaveProductKey(ProductKey productKey);
        ProductKey DeleteProductKey(int productKeyID);
        ProductKey GetProductKeyById(int keyId);

        //ProductOrderEntry

        //IEnumerable<ProductOrderEntry> ProductOrderEntries { get; }
        //void SaveProductOrderEntry(ProductOrderEntry orderProduct);
        //ProductOrderEntry DeleteProductOrderEntry(int orderID);

        //ProductReview

        IEnumerable<ProductReview> ProductReviews { get; }
        void SaveProductReview(ProductReview productReview);
        ProductReview DeleteProductReview(int productReviewID);

        //ShoppingCartEntry

        IEnumerable<ShoppingCartEntry> ShoppingCartEntries { get; }
        void SaveShoppingCartEntry(ShoppingCartEntry shoppingCartEntry);
        ShoppingCartEntry DeleteShoppingCartEntry(int shoppingCartEntryID);

        //Tag

        IEnumerable<Tag> Tags { get; }
        void SaveTag(Tag tag);
        Tag DeleteTag(int id);

        //UserCoupon

        IEnumerable<UserCoupon> UserCoupons { get; }
        void SaveUserCoupon(UserCoupon UserCoupon);
        UserCoupon DeleteUserCoupon(int id);

        //UserNotification

        IEnumerable<UserNotification> UserNotifications { get; }
        void SaveUserNotification(UserNotification userNotification);
        UserNotification DeleteUserNotification(int userNotificationID);

        //WishlistEntry

        IEnumerable<WishlistEntry> WishlistEntries { get; }
        void SaveWishlistEntry(WishlistEntry wishlistEntry);
        WishlistEntry DeleteWishlistEntry(int listingID, int UserID);

        //WonPrize

        IEnumerable<WonPrize> WonPrizes { get; }
        void SaveWonPrize(WonPrize wonPrize);


    }
}
