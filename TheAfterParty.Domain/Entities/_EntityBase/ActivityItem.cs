using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAfterParty.Domain.Entities
{
    public class ActivityItem
    {
        // The basis for ActivityItem is to populate an activity feed, any item that inherits ActivityItem needs a corresponding date to sort by
        public DateTime UpdateDate { get; set; }
    }
}
