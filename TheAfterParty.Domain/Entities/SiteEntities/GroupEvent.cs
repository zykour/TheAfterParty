using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAfterParty.Domain.Entities
{
    public class GroupEvent
    {
        public int GroupEventID { get; set; }
        public bool IsGameNight { get; set; }
        public Product Product { get; set; }
        public int? ProductID { get; set; }
        public void AddProduct(Product product)
        {
            Product = product;
            
            if (product.GroupEvents == null)
            {
                product.GroupEvents = new HashSet<GroupEvent>();
            }

            product.GroupEvents.Add(this);
        }
        public string EventName { get; set; }
        public string Name()
        {
            if (Product != null && String.IsNullOrEmpty(Product.ProductName) == false)
            {
                return Product.ProductName;
            }
            else
            {
                return EventName;
            }    
        }
        public DateTime EventDate { get; set; }
        public DateTime EventCreatedTime { get; set; }
        public string Description { get; set; }
        // in most cases, whoever set up the Steam event
        public virtual AppUser AppUser { get; set; }
        public string AppUserID { get; set; }
    }
}
