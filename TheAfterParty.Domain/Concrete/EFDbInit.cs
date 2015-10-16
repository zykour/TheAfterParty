using System.Data.Entity;
using TheAfterParty.Domain.Concrete;

public class EFDbInit : DropCreateDatabaseIfModelChanges<EFDbContext>
{
    protected override void Seed(EFDbContext context)
    {
        PerformInitialSetup(context);
        base.Seed(context);
    }
    public void PerformInitialSetup(EFDbContext context)
    {
        context.Products.Add(new TheAfterParty.Domain.Entities.Product { AppID = 45000, ItemName = "Sol_Survivor", Platform = 1 });
        context.Products.Add(new TheAfterParty.Domain.Entities.Product { AppID = 18820, ItemName = "Zero_Gear", Platform = 1 });

        context.SaveChanges();
    }
}