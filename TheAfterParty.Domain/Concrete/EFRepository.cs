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
                    targetBalanceEntry.EarnedPoints = balanceEntry.EarnedPoints;
                    targetBalanceEntry.Notes = balanceEntry.Notes;
                    targetBalanceEntry.SteamID = balanceEntry.SteamID;
                    targetBalanceEntry.UpdateDate = balanceEntry.UpdateDate;
                }
            }

            context.SaveChanges();
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
                    targetObjective.Games = objective.Games;
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

        public IEnumerable<OrderProduct> OrderProducts
        {
            get { return context.OrderProducts;  }
        }

        public void SaveOrderProduct(OrderProduct orderProduct)
        {
            if (orderProduct.TransactionID == 0)
            {
                return;
            }

            if (orderProduct.OrderID == 0)
            {
                context.OrderProducts.Add(orderProduct);
            }
            else
            {
                OrderProduct targetOrderProduct = context.OrderProducts.Find(orderProduct.TransactionID, orderProduct.OrderID);

                if (targetOrderProduct != null)
                {
                    targetOrderProduct.DateAdded = orderProduct.DateAdded;
                    targetOrderProduct.ProductKey = orderProduct.ProductKey;
                    targetOrderProduct.SalePrice = orderProduct.SalePrice;
                    targetOrderProduct.StoreID = orderProduct.StoreID;
                }
            }

            context.SaveChanges();
        }

        public OrderProduct DeleteOrderProduct(int transactionID, int orderProductID)
        {
            OrderProduct targetOrderProduct = context.OrderProducts.Find(transactionID, orderProductID);

            if (targetOrderProduct != null)
            {
                context.OrderProducts.Remove(targetOrderProduct);
                context.SaveChanges();

                return targetOrderProduct;
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
                    targetOrder.AppUser = order.AppUser;
                    targetOrder.IsActive = order.IsActive;
                    targetOrder.OrderProducts = order.OrderProducts;
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
        
        public IEnumerable<ProductKey> ProductKeys
        {
            get { return context.ProductKeys; }
        }

        public void SaveProductKey(ProductKey productKey)
        {
            if (productKey.StoreID == 0)
            {
                return;
            }
            else
            {
                ProductKey targetProductKey = context.ProductKeys.Find(productKey.StoreID, productKey.ItemKey);

                if (targetProductKey != null)
                {
                    targetProductKey.Platform = productKey.Platform;
                    targetProductKey.ItemKey = productKey.ItemKey;
                    targetProductKey.IsSold = productKey.IsSold;
                }
                else
                {
                    context.ProductKeys.Add(productKey);
                }
            }

            context.SaveChanges();
        }

        public ProductKey DeleteProductKey(int productKeyID, string itemKey)
        {
            ProductKey targetProductKey = context.ProductKeys.Find(productKeyID, itemKey);

            if (targetProductKey != null)
            {
                context.ProductKeys.Remove(targetProductKey);
                context.SaveChanges();

                return targetProductKey;
            }

            return null;
        }

        public IEnumerable<DiscountedProduct> DiscountedProducts
        {
            get { return context.DiscountedProducts; }
        }

        public void SaveDiscountedProduct(DiscountedProduct discountedProduct)
        {
            if (discountedProduct.StoreID == 0)
            {
                return;
            }
            else
            {
                DiscountedProduct targetDiscountedProduct = context.DiscountedProducts.Find(discountedProduct.StoreID);

                if (targetDiscountedProduct != null)
                {
                    targetDiscountedProduct.ItemDiscountedPrice = discountedProduct.ItemDiscountedPrice;
                    targetDiscountedProduct.ItemDiscountPercent = discountedProduct.ItemDiscountPercent;
                    targetDiscountedProduct.ItemSaleExpiry = discountedProduct.ItemSaleExpiry;
                    targetDiscountedProduct.StockedProduct = discountedProduct.StockedProduct;
                }
                else
                {
                    context.DiscountedProducts.Add(discountedProduct);
                }
            }

            context.SaveChanges();
        }

        public DiscountedProduct DeleteDiscountedProduct(int discountedProductID)
        {
            DiscountedProduct targetDiscountedProduct = context.DiscountedProducts.Find(discountedProductID);

            if (targetDiscountedProduct != null)
            {
                context.DiscountedProducts.Remove(targetDiscountedProduct);
                context.SaveChanges();

                return targetDiscountedProduct;
            }

            return null;
        }

        public IEnumerable<Product> Products
        {
            get { return context.Products;  }
        }

        public void SaveProduct(Product product)
        {
            if (product.StoreID == 0)
            {
                context.Products.Add(product);
            }
            else
            {
                Product targetProduct = context.Products.Find(product.StoreID);

                if (targetProduct != null)
                {
                    targetProduct.AppID = product.AppID;
                    targetProduct.ItemName = product.ItemName;
                    targetProduct.Platform = product.Platform;
                    targetProduct.StockedProduct = product.StockedProduct;
                }
            }

            context.SaveChanges();
        }

        public IEnumerable<StockedProduct> StockedProducts
        {
            get { return context.StockedProducts; }
        }

        public void SaveStockedProduct(StockedProduct stockedProduct)
        {
            if (stockedProduct.StoreID == 0)
            {
                return;
            }
            else
            {
                StockedProduct targetStockedProduct = context.StockedProducts.Find(stockedProduct.StoreID);

                if (targetStockedProduct != null)
                {
                    targetStockedProduct.Product = stockedProduct.Product;
                    targetStockedProduct.ProductKeys = stockedProduct.ProductKeys;
                    targetStockedProduct.DiscountedProduct = stockedProduct.DiscountedProduct;

                    targetStockedProduct.ItemPrice = stockedProduct.ItemPrice;
                    targetStockedProduct.ItemQuantity = stockedProduct.ItemQuantity;
                    targetStockedProduct.Platform = stockedProduct.Platform;
                }
                else
                {
                    context.StockedProducts.Add(stockedProduct);
                }
            }

            context.SaveChanges();
        }

        public StockedProduct DeleteStockedProduct(int stockedProductID)
        {
            StockedProduct targetStockedProduct = context.StockedProducts.Find(stockedProductID);

            if (targetStockedProduct != null)
            {
                context.StockedProducts.Remove(targetStockedProduct);
                context.SaveChanges();

                return targetStockedProduct;
            }

            return null;
        }

    }
}
