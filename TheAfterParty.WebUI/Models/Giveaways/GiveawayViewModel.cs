using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models._Nav;

namespace TheAfterParty.WebUI.Models.Giveaways
{
    public class GiveawayViewModel : NavModel
    {
        public Giveaway Giveaway { get; set; }

        public string Title { get; set; }

        public string ActionName { get; set; }
    }
}