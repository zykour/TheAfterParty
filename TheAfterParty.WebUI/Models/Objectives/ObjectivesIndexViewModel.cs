using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Entities;
using TheAfterParty.WebUI.Models.Store;

namespace TheAfterParty.WebUI.Models.Objectives
{
    public class ObjectivesIndexViewModel : NavModel
    {
        public ObjectivesIndexViewModel()
        {
            Objectives = new List<Objective>();
            SelectedTagMappings = new List<SelectedTagMapping>();

            TagToChange = 0;
            PreviousFilterLibrary = false;
            FilterLibrary = false;
        }

        public List<Objective> Objectives { get; set; }

        public List<SelectedTagMapping> SelectedTagMappings { get; set; }
        public int TagToChange { get; set; }
        public bool PreviousFilterLibrary { get; set; }
        public bool FilterLibrary { get; set; }
    }
}