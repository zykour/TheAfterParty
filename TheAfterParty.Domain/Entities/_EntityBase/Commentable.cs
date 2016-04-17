using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAfterParty.Domain.Entities
{
    public class Commentable
    {
        public virtual ICollection<ListingComment> Comments { get; set; }
        public void AddComment(ListingComment comment)
        {
            if (Comments == null)
            {
                Comments = new HashSet<ListingComment>();
            }

            Comments.Add(comment);
        }
    }
}
