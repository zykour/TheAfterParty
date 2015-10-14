using System.Collections.Generic;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Abstract
{
    public interface IRepository
    {
        IEnumerable<Product> Products { get; }
        void SaveProduct(Product product);

        IEnumerable<StockedProduct> StockedProducts { get; }
        void SaveStockedProduct(StockedProduct stockedProduct);
        StockedProduct DeleteStockedProduct(int stockedProductID);

        IEnumerable<DiscountedProduct> DiscountedProducts { get; }
        void SaveDiscountedProduct(DiscountedProduct discountedProduct);
        DiscountedProduct DeleteDiscountedProduct(int discountedProductID);

        IEnumerable<ProductKey> ProductKeys { get; }
        void SaveProductKey(ProductKey productKey);
        ProductKey DeleteProductKey(int productKeyID);


        IEnumerable<AppUser> AppUsers { get; }
        void SaveAppUser(AppUser appUsers);

        IEnumerable<Order> Orders { get; }
        void SaveOrder(Order order);
        Order DeleteOrder(int orderID);

        IEnumerable<OrderProduct> OrderProducts { get; }
        void SaveOrderProduct(OrderProduct orderProduct);
        OrderProduct DeleteOrderProduct(int transactionID, int orderProductID);


        IEnumerable<Objective> Objectives { get; }
        void SaveObjective(Objective objective);
        Objective DeleteObjective(int objectiveID);
    }
}
