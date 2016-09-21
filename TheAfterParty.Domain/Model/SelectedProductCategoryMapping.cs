using TheAfterParty.Domain.Entities;
namespace TheAfterParty.Domain.Model
{
    public class SelectedProductCategoryMapping
    {
        public SelectedProductCategoryMapping() { }
        public SelectedProductCategoryMapping(ProductCategory category, bool selected)
        {
            ProductCategory = category;
            IsSelected = selected;
        }
        public ProductCategory ProductCategory { get; set; }
        public bool IsSelected { get; set; }
    }
}