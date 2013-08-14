using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JerryChat.Services.Models
{
    public class UserRegister
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AvatarUrl { get; set; }
        public string SessionKey { get; set; }
    }
}