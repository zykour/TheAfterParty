using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Store
{
    public class SelectedTagMapping : NavModel
    {
        public SelectedTagMapping() { }
        public SelectedTagMapping(Tag tag, bool selected)
        {
            StoreTag = tag;
            IsSelected = selected;
        }
        public Tag StoreTag { get; set; }
        public bool IsSelected { get; set; }
    }
}