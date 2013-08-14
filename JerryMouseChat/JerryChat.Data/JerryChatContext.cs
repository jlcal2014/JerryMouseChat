using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JerryChat.Models;

namespace JerryChat.Data
{
    public class JerryChatContext: DbContext
    {
        public JerryChatContext()
            : base("Default")
        {

        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}
