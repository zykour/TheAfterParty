using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheAfterParty.Domain.Entities;

namespace TheAfterParty.WebUI.Models.Api
{
    public class ApiAppUserModel
    {
        public ApiAppUserModel()
        {
            UserName = String.Empty;
            NickName = String.Empty;
        }
        public ApiAppUserModel(AppUser user)
        {
            SteamID = user.UserSteamID;
            Balance = user.Balance;
            UserName = user.UserName;
            NickName = user.Nickname;
        }

        public long SteamID { get; set; }
        public int Balance { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
    }
}