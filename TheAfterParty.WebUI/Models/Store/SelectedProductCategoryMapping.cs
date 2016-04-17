using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Store
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