using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.Domain.Model
{
    public class StoreIndexDomainModel
    {
        public IEnumerable<Platform> StorePlatforms { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
    }
}
