using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TheAfterParty.Domain.Entities
{
    public class ProductCategory
    {
        public ProductCategory() { }

        [Key]
        public int ProductCategoryID { get; set; }

        public string CategoryString { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
