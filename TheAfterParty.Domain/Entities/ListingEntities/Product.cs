﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

namespace TheAfterParty.Domain.Entities
{

    // Table to hold data for all items ever sold in the store, may include items that have never been sold on the store too.

    // This table is a superset of the CurrentSaleItem table

    public class Product
    {
        public Product() { }
        public Product(int appId)
        {
            this.AppID = appId;
        }
        public Product(int appId, string productName)
        {
            this.AppID = appId;
            this.ProductName = productName;
        }

        // id of an item in the store's database, many other entities rely on this key
        [Key]
        public int ProductID { get; set; }
        
        // if the corresponding platform has a publicly displayed application/game ID, this is stored here
        public int AppID { get; set; }

        public string ProductName { get; set; }
        
        public virtual ICollection<Listing> Listings { get; set; }

        // steam categories, i.e. Co-op, Multiplayer, Steam Trading Cards, Partial Controller Support, Steam Cloud
        //public string Categories { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }

        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        public int TotalReviews()
        {
            return ProductReviews.Count();
        }
        public int PositiveReviews()
        {
            return ProductReviews.Where(r => r.IsRecommended == true).Count();
        }

        public virtual ICollection<Objective> Objectives { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
        public bool HasTag(Tag tag)
        {
            return (Tags.Where(t => object.Equals(tag.TagName, t.TagName)).Count() >= 1) ? true : false;
        }
                
        public virtual ProductDetail ProductDetail { get; set; }
        public void AddProductDetail(ProductDetail productDetail)
        {
            productDetail.Product = this;
            ProductDetail = productDetail;
        }
    }
}