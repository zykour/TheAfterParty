using System.Collections.Generic;
using TheAfterParty.Domain.Abstract;
using TheAfterParty.Domain.Entities;

// fix the inserts so they grab the right ID
// touch up insertions in general to make sure all supplemental tables contain relevant data regardless of which table gets the first insert for a new item

namespace TheAfterParty.Domain.Concrete
{
    public class EFRepository : IRepository
    {
        private EFDbContext context = new EFDbContext();

        public IEnumerable<Auction> Auctions
        {
            get { return context.Auctions;  }
        }

        public void SaveAuction(Auction auction)
        {
            if (auction.AuctionID == 0)
            {
                context.Auctions.Add(auction);
            }
            else
            {
                Auction targetAuction = context.Auctions.Find(auction.AuctionID);

                if (targetAuction != null)
                {
                    targetAuction.AlternativePrize = auction.AlternativePrize;
                    targetAuction.EndTime = auction.EndTime;
                    targetAuction.IsSilent = auction.IsSilent;
                    targetAuction.MinimumBid = auction.MinimumBid;
                    targetAuction.WinnerID = auction.WinnerID;
                    targetAuction.ListingID = auction.ListingID;
                }
            }

            context.SaveChanges();
        }

        public IEnumerable<AuctionBid> AuctionBids
        {
            get { return context.AuctionBids;  }
        }

        public void SaveAuctionBid(AuctionBid auctionBid)
        {
            if (auctionBid.BidNumber == 0)
            {
                context.AuctionBids.Add(auctionBid);
            }
            else
            {
                AuctionBid targetAuctionBid = context.AuctionBids.Find(auctionBid.BidNumber);

                if (targetAuctionBid != null)
                {
                    targetAuctionBid.BidAmount = auctionBid.BidAmount;
                    targetAuctionBid.BidDate = auctionBid.BidDate;
                }
            }

            context.SaveChanges();
        }

        public AuctionBid DeleteAuctionBid(int auctionID, int bidNumber)
        {
            AuctionBid targetAuctionBid = context.AuctionBids.Find(auctionID, bidNumber);

            if (targetAuctionBid != null)
            {
                context.AuctionBids.Remove(targetAuctionBid);
                context.SaveChanges();

                return targetAuctionBid;
            }

            return null;
        }

        public IEnumerable<BalanceEntry> BalanceEntries
        {
            get { return context.BalanceEntries; }
        }

        public void SaveBalanceEntry(BalanceEntry balanceEntry)
        {
            if (balanceEntry.BalanceID == 0)
            {
                context.BalanceEntries.Add(balanceEntry);
            }
            else
            {
                BalanceEntry targetBalanceEntry = context.BalanceEntries.Find(balanceEntry.BalanceID);

                if (targetBalanceEntry != null)
                {
                    targetBalanceEntry.PointsAdjusted = balanceEntry.PointsAdjusted;
                    targetBalanceEntry.Notes = balanceEntry.Notes;
                    targetBalanceEntry.UserID = balanceEntry.UserID;
                    targetBalanceEntry.Date = balanceEntry.Date;
                }
            }

            context.SaveChanges();
        }

        public IEnumerable<BoostedObjective> BoostedObjectives
        {
            get { return context.BoostedObjectives; }
        }

        public void SaveBoostedObjective(BoostedObjective boostedObjecive)
        {
            BoostedObjective targetBoostedObjective = context.BoostedObjectives.Find(boostedObjecive.ObjectiveID);

            if (targetBoostedObjective == null)
            {
                context.BoostedObjectives.Add(boostedObjecive);
            }
            else
            {
                targetBoostedObjective.BoostAmount = boostedObjecive.BoostAmount;
                targetBoostedObjective.EndDate = boostedObjecive.EndDate;
            }
            
            context.SaveChanges();
        }

        public BoostedObjective DeleteBoostedObjective(int objectiveID)
        {
            BoostedObjective targetBoostedObjective = context.BoostedObjectives.Find(objectiveID);

            if (targetBoostedObjective != null)
            {
                context.BoostedObjectives.Remove(targetBoostedObjective);
                context.SaveChanges();

                return targetBoostedObjective;
            }
            
            return null;
        }

        public IEnumerable<ClaimedProductKey> ClaimedProductKeys
        {
            get { return context.ClaimedProductKeys; }
        }

        public void SaveClaimedProductKey(ClaimedProductKey claimedProductKey)
        {
            if (claimedProductKey.KeyID == 0)
            {
                context.ClaimedProductKeys.Add(claimedProductKey);
            }
            else
            {
                ClaimedProductKey targetClaimedProductKey = context.ClaimedProductKeys.Find(claimedProductKey.KeyID);

                if (targetClaimedProductKey != null)
                {
                    targetClaimedProductKey.AcquisitionTitle = claimedProductKey.AcquisitionTitle;
                    targetClaimedProductKey.Date = claimedProductKey.Date;
                    targetClaimedProductKey.IsRevealed = claimedProductKey.IsRevealed;
                    targetClaimedProductKey.IsUsed = claimedProductKey.IsUsed;
                    targetClaimedProductKey.Key = claimedProductKey.Key;
                    targetClaimedProductKey.ListingID = claimedProductKey.ListingID;
                    targetClaimedProductKey.UserID = claimedProductKey.UserID;
                    targetClaimedProductKey.IsGift = claimedProductKey.IsGift;
                }
            }

            context.SaveChanges();
        }

        public ClaimedProductKey DeleteClaimedProductKey(int keyID)
        {
            ClaimedProductKey targetClaimedProductKey = context.ClaimedProductKeys.Find(keyID);

            if (targetClaimedProductKey != null)
            {
                context.ClaimedProductKeys.Remove(targetClaimedProductKey);
                context.SaveChanges();

                return targetClaimedProductKey;
            }

            return null;
        }

        public IEnumerable<Coupon> Coupons
        {
            get { return context.Coupons; }
        }

        public void SaveCoupon(Coupon coupon)
        {
            if (coupon.CouponID == 0)
            {
                context.Coupons.Add(coupon);
            }
            else
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
            }

            context.SaveChanges();
        }

        public IEnumerable<DiscountedListing> DiscountedListings
        {
            get { return context.DiscountedListings; }
        }

        public void SaveDiscountedListing(DiscountedListing discountedListing)
        {
            DiscountedListing targetDiscountedListing = context.DiscountedListings.Find(discountedListing.ListingID);

            if (targetDiscountedListing == null)
            {
                context.DiscountedListings.Add(discountedListing);
            }
            else
            {
                targetDiscountedListing.ItemDiscountedPrice = discountedListing.ItemDiscountedPrice;
                targetDiscountedListing.ItemDiscountPercent = discountedListing.ItemDiscountPercent;
                targetDiscountedListing.ItemSaleExpiry = discountedListing.ItemSaleExpiry;
            }

            context.SaveChanges();
        }

        public DiscountedListing DeleteDiscountedListing(int discountedListingID)
        {
            DiscountedListing targetDiscountedListing = context.DiscountedListings.Find(discountedListingID);

            if (targetDiscountedListing != null)
            {
                context.DiscountedListings.Remove(targetDiscountedListing);
                context.SaveChanges();

                return targetDiscountedListing;
            }

            return null;
        }

        public IEnumerable<Gift> Gifts
        {
            get { return context.Gifts; }
        }

        public void SaveGift(Gift gift)
        {
            Gift targetGift = context.Gifts.Find(gift.GiftID);

            if (targetGift == null)
            {
                context.Gifts.Add(gift);
            }
            else
            {
                targetGift.DateReceived = gift.DateReceived;
                targetGift.DateSent = gift.DateSent;
                targetGift.IsPending = gift.IsPending;
                targetGift.ReceiverID = gift.ReceiverID;
                targetGift.SenderID = gift.SenderID;
            }
            
            context.SaveChanges();
        }

        public Gift DeleteGift(int giftID)
        {
            Gift targetGift = context.Gifts.Find(giftID);

            if (targetGift != null)
            {
                context.Gifts.Remove(targetGift);                
                context.SaveChanges();

                return targetGift;
            }

            return null;
        }

        public IEnumerable<Giveaway> Giveaways
        {
            get { return context.Giveaways; }
        }

        public void SaveGiveaway(Giveaway giveaway)
        {
            if (giveaway.GiveawayID == 0)
            {
                context.Giveaways.Add(giveaway);
            }
            else
            {
                Giveaway targetGiveaway = context.Giveaways.Find(giveaway.GiveawayID);

                if (targetGiveaway != null)
                {
                    targetGiveaway.EndDate = giveaway.EndDate;
                    targetGiveaway.EntryFee = giveaway.EntryFee;
                    targetGiveaway.ListingID = giveaway.ListingID;
                    targetGiveaway.PointsPrize = giveaway.PointsPrize;
                    targetGiveaway.Prize = giveaway.Prize;
                    targetGiveaway.StartDate = giveaway.StartDate;
                    targetGiveaway.UserID = giveaway.UserID;
                }
            }

            context.SaveChanges();
        }

        public Giveaway DeleteGiveaway(int giveawayID)
        {
            Giveaway targetGiveaway = context.Giveaways.Find(giveawayID);

            if (targetGiveaway != null)
            {
                context.Giveaways.Remove(targetGiveaway);
                context.SaveChanges();

                return targetGiveaway;
            }

            return null;
        }

        public IEnumerable<GiveawayEntry> GiveawayEntries
        {
            get { return context.GiveawayEntries; }
        }

        public void SaveGiveawayEntry(GiveawayEntry giveawayEntry)
        {
            if (giveawayEntry.EntryNumber == 0)
            {
                context.GiveawayEntries.Add(giveawayEntry);
            }
            else
            {
                GiveawayEntry targetGiveawayEntry = context.GiveawayEntries.Find(giveawayEntry.GiveawayID, giveawayEntry.EntryNumber);
                
                if (giveawayEntry != null)
                {
                    targetGiveawayEntry.EntryDate = giveawayEntry.EntryDate;
                    targetGiveawayEntry.HasDonated = giveawayEntry.HasDonated;
                    targetGiveawayEntry.UserID = giveawayEntry.UserID;
                }               
            }

            context.SaveChanges();
        }

        public GiveawayEntry DeleteGiveawayEntry(int giveawayID, int entryNumber)
        {
            GiveawayEntry targetGiveawayEntry = context.GiveawayEntries.Find(giveawayID, entryNumber);

            if (targetGiveawayEntry != null)
            {
                context.GiveawayEntries.Remove(targetGiveawayEntry);
                context.SaveChanges();

                return targetGiveawayEntry;
            }

            return null;
        }

        public IEnumerable<Listing> Listings
        {
            get { return context.Listings; }
        }

        public Listing GetListingById(int listingId)
        {
            return context.Listings.Find(listingId);            
        }

        public void SaveListing(Listing listing)
        {
            if (listing.ListingID == 0)
            {
                context.Listings.Add(listing);
            }
            else
            {
                Listing targetListing = context.Listings.Find(listing.ListingID);

                if (targetListing != null)
                {
                    targetListing.ListingName = listing.ListingName;
                    targetListing.ListingPrice = listing.ListingPrice;
                    targetListing.Quantity = listing.Quantity;
                }
            }

            context.SaveChanges();
        }

        /*public Listing DeleteListing(int listingID)
        {
            Listing targetListing = context.Listings.Find(listingID);

            if (targetListing != null)
            {
                context.Listings.Remove(targetListing);
                context.SaveChanges();

                return targetListing;
            }

            return null;
        }*/

        public IEnumerable<Mail> Mail
        {
            get { return context.Mail; }
        }

        public void SaveMail(Mail mail)
        {
            if (mail.MailID == 0)
            {
                context.Mail.Add(mail);
            }
            else
            {
                Mail targetMail = context.Mail.Find(mail.MailID);

                if (targetMail != null)
                {
                    targetMail.DateSent = mail.DateSent;
                    targetMail.Message = mail.Message;
                    targetMail.ReceiverUserID = mail.ReceiverUserID;
                    targetMail.SenderUserID = mail.SenderUserID;
                }
            }

            context.SaveChanges();
        }

        public Mail DeleteMail(int mailID)
        {
            Mail targetMail = context.Mail.Find(mailID);

            if (targetMail != null)
            {
                context.Mail.Remove(targetMail);
                context.SaveChanges();

                return targetMail;
            }

            return null;
        }

        public IEnumerable<MappedListing> MappedListings
        {
            get { return context.MappedListings; }
        }

        public void SaveMappedListing(MappedListing mappedListing)
        {
            if (mappedListing.Id == 0)
            {
                context.MappedListings.Add(mappedListing);
            }

            context.SaveChanges();
        }

        public MappedListing DeleteMappedListing(int id)
        {
            MappedListing targetMappedListing = context.MappedListings.Find(id);

            if (targetMappedListing != null)
            {
                context.MappedListings.Remove(targetMappedListing);
                context.SaveChanges();

                return targetMappedListing;
            }

            return null;
        }
        
        public IEnumerable<Objective> Objectives
        {
            get { return context.Objectives; }
        }

        public void SaveObjective(Objective objective)
        {
            if (objective.ObjectiveID == 0)
            {
                context.Objectives.Add(objective);
            }
            else
            {
                Objective targetObjective = context.Objectives.Find(objective.ObjectiveID);

                if (targetObjective != null)
                {
                    targetObjective.Description = objective.Description;
                    targetObjective.Category = objective.Category;
                    targetObjective.RequiresAdmin = objective.RequiresAdmin;
                    targetObjective.Reward = objective.Reward;
                }
            }

            context.SaveChanges();
        }

        public Objective DeleteObjective(int objectiveID)
        {
            Objective targetObjective = context.Objectives.Find(objectiveID);

            if (targetObjective != null)
            {
                context.Objectives.Remove(targetObjective);
                context.SaveChanges();

                return targetObjective;
            }

            return null;
        }

        public IEnumerable<ObjectiveGameMapping> ObjectiveGameMappings
        {
            get { return context.ObjectiveGameMappings; }
        }

        public void SaveObjectiveGameMapping(ObjectiveGameMapping objectiveGameMapping)
        {
            if (objectiveGameMapping.Id == 0)
            {
                context.ObjectiveGameMappings.Add(objectiveGameMapping);
            }

            context.SaveChanges();
        }

        public ObjectiveGameMapping DeleteObjectiveGameMapping(int id)
        {
            ObjectiveGameMapping targetObjectiveGameMapping = context.ObjectiveGameMappings.Find(id);

            if (targetObjectiveGameMapping != null)
            {
                context.ObjectiveGameMappings.Remove(targetObjectiveGameMapping);
                context.SaveChanges();

                return targetObjectiveGameMapping;
            }

            return null;
        }

        public IEnumerable<Order> Orders
        {
            get { return context.Orders; }
        }

        public void SaveOrder(Order order)
        {
            if (order.TransactionID == 0)
            {
                context.Orders.Add(order);
            }
            else
            {
                Order targetOrder = context.Orders.Find(order.TransactionID);

                if (targetOrder != null)
                {
                    targetOrder.SaleDate = order.SaleDate;
                    targetOrder.UserID = order.UserID;
                }
            }

            context.SaveChanges();
        }

        public Order DeleteOrder(int orderID)
        {
            Order targetOrder = context.Orders.Find(orderID);

            if (targetOrder != null)
            {
                context.Orders.Remove(targetOrder);
                context.SaveChanges();

                return targetOrder;
            }

            return null;
        }

        public IEnumerable<OwnedGame> OwnedGames
        {
            get { return context.OwnedGames; }
        }

        public void SaveOwnedGame(OwnedGame ownedGame)
        {
            if (ownedGame.Id == 0)
            {
                context.OwnedGames.Add(ownedGame);
            }

            context.SaveChanges();
        }

        public IEnumerable<Prize> Prizes
        {
            get { return context.Prizes; }
        }

        public void SavePrize(Prize prize)
        {
            Prize targetPrize = context.Prizes.Find(prize.PrizeID);

            if (targetPrize == null)
            {
                context.Prizes.Add(prize);
            }
            else
            {
                targetPrize.ChancePerThousand = prize.ChancePerThousand;
                targetPrize.Description = prize.Description;
                targetPrize.IsAvailable = prize.IsAvailable;
                targetPrize.Tier = prize.Tier;
            }

            context.SaveChanges();
        }

        public IEnumerable<Product> Products
        {
            get { return context.Products; }
        }

        public void SaveProduct(Product product)
        {
            Product targetProduct = context.Products.Find(product.ProductID);

            if (targetProduct == null)
            {
                context.Products.Add(product);
            }
            else
            {
                targetProduct.AppID = product.AppID;
                targetProduct.ProductName = product.ProductName;
                targetProduct.Platform = product.Platform;
            }

            context.SaveChanges();
        }

        public IEnumerable<ListingComment> ListingComments
        {
            get { return context.ListingComments; }
        }

        public void SaveListingComment(ListingComment ListingComment)
        {
            if (ListingComment.ListingCommentID == 0)
            {
                context.ListingComments.Add(ListingComment);
            }
            else
            {
                ListingComment targetListingComment = context.ListingComments.Find(ListingComment.ListingCommentID);

                if (targetListingComment != null)
                {
                    targetListingComment.Comment = ListingComment.Comment;
                    targetListingComment.IsEdited = ListingComment.IsEdited;
                    targetListingComment.LastEdited = ListingComment.LastEdited;
                    targetListingComment.ListingID = ListingComment.ListingID;
                    targetListingComment.PostDate = ListingComment.PostDate;
                    targetListingComment.UserID = ListingComment.UserID;
                }
            }

            context.SaveChanges();
        }

        public ListingComment DeleteListingComment(int ListingCommentID)
        {
            ListingComment targetListingComment = context.ListingComments.Find(ListingCommentID);

            if (targetListingComment != null)
            {
                context.ListingComments.Remove(targetListingComment);
                context.SaveChanges();

                return targetListingComment;
            }

            return null;
        }

        public IEnumerable<ProductDetail> ProductDetails
        {
            get { return context.ProductDetails; }
        }

        public void SaveProductDetail(ProductDetail productDetail)
        {
            ProductDetail targetProductDetail = context.ProductDetails.Find(productDetail.ProductID);

            if (targetProductDetail == null)
            {
                context.ProductDetails.Add(targetProductDetail);
            }
            else
            {
                targetProductDetail.ImageData = productDetail.ImageData;
                targetProductDetail.ImageMimeType = productDetail.ImageMimeType;
                targetProductDetail.ProductName = productDetail.ProductName;
            }

            context.SaveChanges();
        }

        public ProductDetail DeleteProductDetail(int productId)
        {
            ProductDetail targetProductDetail = context.ProductDetails.Find(productId);

            if (targetProductDetail != null)
            {
                context.ProductDetails.Remove(targetProductDetail);
                context.SaveChanges();

                return targetProductDetail;
            }

            return null;
        }

        public IEnumerable<ProductKey> ProductKeys
        {
            get { return context.ProductKeys; }
        }

        public ProductKey GetProductKeyById(int keyId)
        {
            return context.ProductKeys.Find(keyId);            
        }

        public void SaveProductKey(ProductKey productKey)
        {
            if (productKey.KeyID == 0)
            {
                return;
            }
            else
            {
                ProductKey targetProductKey = context.ProductKeys.Find(productKey.KeyID, productKey.ItemKey);

                if (targetProductKey != null)
                {
                    targetProductKey.ListingID = productKey.ListingID;
                    targetProductKey.ItemKey = productKey.ItemKey;
                    targetProductKey.IsGift = productKey.IsGift;
                }
                else
                {
                    context.ProductKeys.Add(productKey);
                }
            }

            context.SaveChanges();
        }

        public ProductKey DeleteProductKey(int productKeyID)
        {
            ProductKey targetProductKey = context.ProductKeys.Find(productKeyID);

            if (targetProductKey != null)
            {
                context.ProductKeys.Remove(targetProductKey);
                context.SaveChanges();

                return targetProductKey;
            }

            return null;
        }

        public IEnumerable<ProductOrderEntry> ProductOrderEntries
        {
            get { return context.ProductOrderEntries;  }
        }

        public void SaveProductOrderEntry(ProductOrderEntry orderProduct)
        {
            ProductOrderEntry targetOrderProduct = context.ProductOrderEntries.Find(orderProduct.OrderID);

            if (targetOrderProduct == null)
            {
                context.ProductOrderEntries.Add(orderProduct);
            }
            else
            {
                targetOrderProduct.KeyID = orderProduct.KeyID;
                targetOrderProduct.SalePrice = orderProduct.SalePrice;
                targetOrderProduct.ListingID = orderProduct.ListingID;
            }

            context.SaveChanges();
        }

        public ProductOrderEntry DeleteProductOrderEntry(int orderID)
        {
            ProductOrderEntry targetOrderProduct = context.ProductOrderEntries.Find(orderID);

            if (targetOrderProduct != null)
            {
                context.ProductOrderEntries.Remove(targetOrderProduct);
                context.SaveChanges();

                return targetOrderProduct;
            }

            return null;
        }

        public IEnumerable<ProductReview> ProductReviews
        {
            get { return context.ProductReviews; }
        }

        public void SaveProductReview(ProductReview productReview)
        {
            if (productReview.ProductReviewID == 0)
            {
                context.ProductReviews.Add(productReview);
            }
            else
            {
                ProductReview targetProductReview = context.ProductReviews.Find(productReview.ProductReviewID);

                if (targetProductReview != null)
                {
                    targetProductReview.IsEdited = productReview.IsEdited;
                    targetProductReview.IsRecommended = productReview.IsRecommended;
                    targetProductReview.LastEdited = productReview.LastEdited;
                    targetProductReview.ProductID = productReview.ProductID;
                    targetProductReview.PostDate = productReview.PostDate;
                    targetProductReview.Review = productReview.Review;
                    targetProductReview.UserID = productReview.UserID;
                }
            }

            context.SaveChanges();
        }

        public ProductReview DeleteProductReview(int productReviewID)
        {
            ProductReview targetProductReview = context.ProductReviews.Find(productReviewID);

            if (targetProductReview != null)
            {
                context.ProductReviews.Remove(targetProductReview);
                context.SaveChanges();

                return targetProductReview;
            }

            return null;
        }

        public IEnumerable<ShoppingCartEntry> ShoppingCartEntries
        {
            get { return context.ShoppingCartEntries; }
        }

        public void SaveShoppingCartEntry(ShoppingCartEntry shoppingCartEntry)
        {
            if (shoppingCartEntry.ShoppingID == 0)
            {
                context.ShoppingCartEntries.Add(shoppingCartEntry);
            }
            else
            {
                ShoppingCartEntry targetShoppingCartEntry = context.ShoppingCartEntries.Find(shoppingCartEntry.ShoppingID);

                if (targetShoppingCartEntry != null)
                {
                    targetShoppingCartEntry.Quantity = shoppingCartEntry.Quantity;
                    targetShoppingCartEntry.DateAdded = shoppingCartEntry.DateAdded;
                    targetShoppingCartEntry.ListingID = shoppingCartEntry.ListingID;
                    targetShoppingCartEntry.UserID = shoppingCartEntry.UserID;
                }

            }

            context.SaveChanges();
        }

        public ShoppingCartEntry DeleteShoppingCartEntry(int shoppingID)
        {
            ShoppingCartEntry targetShoppingCartEntry = context.ShoppingCartEntries.Find(shoppingID);

            if (targetShoppingCartEntry != null)
            {
                context.ShoppingCartEntries.Remove(targetShoppingCartEntry);
                context.SaveChanges();

                return targetShoppingCartEntry;
            }

            return null;
        }

        public IEnumerable<Tag> Tags
        {
            get { return context.Tags; }
        }

        public void SaveTag(Tag tag)
        {
            if (tag.Id == 0)
            {
                context.Tags.Add(tag);
            }
            else
            {
                Tag targetTag = context.Tags.Find(tag.Id);

                if (targetTag != null)
                {
                    targetTag.TagName = tag.TagName;
                }
            }

            context.SaveChanges();
        }

        public Tag DeleteTag(int id)
        {
            Tag targetTag = context.Tags.Find(id);

            if (targetTag != null)
            {
                context.Tags.Remove(targetTag);
                context.SaveChanges();

                return targetTag;
            }

            return null;
        }

        public IEnumerable<UserCoupon> UserCoupons
        {
            get { return context.UserCoupons; }
        }

        public void SaveUserCoupon(UserCoupon userCoupon)
        {
            if (userCoupon.Id == 0)
            {
                context.UserCoupons.Add(userCoupon);
            }
            else
            {
                UserCoupon targetUserCoupon = context.UserCoupons.Find(userCoupon.Id);

                if (targetUserCoupon != null)
                {
                    targetUserCoupon.Quantity = userCoupon.Quantity;
                }
            }

            context.SaveChanges();
        }

        public UserCoupon DeleteUserCoupon(int Id)
        {
            UserCoupon targetUserCoupon = context.UserCoupons.Find(Id);

            if (targetUserCoupon != null)
            {
                context.UserCoupons.Remove(targetUserCoupon);
                context.SaveChanges();

                return targetUserCoupon;
            }

            return null;
        }

        public IEnumerable<UserNotification> UserNotifications
        {
            get { return context.UserNotifications; }
        }

        public void SaveUserNotification(UserNotification userNotification)
        {
            if (userNotification.UserNotificationID == 0)
            {
                context.UserNotifications.Add(userNotification);
            }
            else
            {
                UserNotification targetUserNotification = context.UserNotifications.Find(userNotification.UserNotificationID);

                if (targetUserNotification != null)
                {
                    targetUserNotification.DateTime = userNotification.DateTime;
                    targetUserNotification.IsRead = userNotification.IsRead;
                    targetUserNotification.Message = userNotification.Message;
                }
            }

            context.SaveChanges();
        }

        public UserNotification DeleteUserNotification(int userNotificationID)
        {
            UserNotification targetUserNotification = context.UserNotifications.Find(userNotificationID);

            if (targetUserNotification != null)
            {
                context.UserNotifications.Remove(targetUserNotification);
                context.SaveChanges();

                return targetUserNotification;
            }

            return null;
        }

        public IEnumerable<WishlistEntry> WishlistEntries
        {
            get { return context.WishlistEntries; }
        }

        public void SaveWishlistEntry(WishlistEntry wishlistEntry)
        {
            WishlistEntry targetWishlistEntry = context.WishlistEntries.Find(wishlistEntry.UserID, wishlistEntry.ListingID);

            if (targetWishlistEntry == null)
            {
                context.WishlistEntries.Add(wishlistEntry);
            }
            
            context.SaveChanges();
        }

        public WishlistEntry DeleteWishlistEntry(int userID, int listingID)
        {
            WishlistEntry targetWishlistEntry = context.WishlistEntries.Find(userID, listingID);

            if (targetWishlistEntry != null)
            {
                context.WishlistEntries.Remove(targetWishlistEntry);
                context.SaveChanges();

                return targetWishlistEntry;
            }

            return null;
        }

        public IEnumerable<WonPrize> WonPrizes
        {
            get { return context.WonPrizes; }
        }

        public void SaveWonPrize(WonPrize wonPrize)
        {
            if (wonPrize.WonPrizeID == 0)
            {
                context.WonPrizes.Add(wonPrize);
            }
            else
            {
                WonPrize targetWonPrize = context.WonPrizes.Find(wonPrize.WonPrizeID);

                if (targetWonPrize != null)
                {
                    targetWonPrize.WinningAction = wonPrize.WinningAction;
                }
            }

            context.SaveChanges();
        }
    }
}
