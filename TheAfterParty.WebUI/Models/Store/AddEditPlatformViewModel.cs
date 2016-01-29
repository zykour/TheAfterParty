using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Store
{
    public class AddEditPlatformViewModel
    {
        public AddEditPlatformViewModel()
        {
            Success = false;
        }

        public Platform Platform { get; set; }
        public bool Success { get; set; }
    }
}