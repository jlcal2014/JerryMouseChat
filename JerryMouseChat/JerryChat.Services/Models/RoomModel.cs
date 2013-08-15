using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JerryChat.Services.Models
{
    public class RoomModel
    {
        public RoomModel()
        {
            this.UserIds = new HashSet<int>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int UsersCount { get; set; }
        public bool IsLocked { get; set; }
        public int AdminId { get; set; }
        public ICollection<int> UserIds { get; set; } 
    }
}