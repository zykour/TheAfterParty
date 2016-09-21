using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAfterParty.Domain.Entities
{
    public class POTW
    {
        public int POTWID { get; set; }
        public virtual AppUser AppUser { get; set; }
        public string AppUserID { get; set; }
        public DateTime StartDate { get; set; }
    }
}
