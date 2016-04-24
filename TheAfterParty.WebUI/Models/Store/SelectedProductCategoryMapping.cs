using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Store
{
    public class SelectedProductCategoryMapping : NavModel
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