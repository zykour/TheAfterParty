using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.WebUI.Models._Nav;
using TheAfterParty.Domain.Entities;
using TheAfterParty.Domain.Model;

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

            FormID = "";
        }

        public string FormName { get; set; }
        public string FormID { get; set; }

        public List<Objective> MiscObjectives { get; set; }
        public List<Objective> Objectives { get; set; }

        public List<SelectedTagMapping> SelectedTagMappings { get; set; }
        public int TagToChange { get; set; }
        public bool PreviousFilterLibrary { get; set; }
        public bool FilterLibrary { get; set; }
    }
}