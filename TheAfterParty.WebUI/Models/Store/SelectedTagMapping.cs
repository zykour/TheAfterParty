using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Store
{
    public class SelectedTagMapping
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