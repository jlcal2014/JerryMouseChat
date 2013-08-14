using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JerryChat.Services.Models
{
    public class UserDetails
    {
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public virtual ICollection<MessageModel> Messages { get; set; }
        public virtual ICollection<RoomModel> Rooms { get; set; }
    }
}