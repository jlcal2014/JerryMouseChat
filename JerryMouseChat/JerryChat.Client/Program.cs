using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JerryChat.Data;
using JerryChat.Models;

namespace JerryChat.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            JerryChatContext ctx = new JerryChatContext();
            User user = new User();
            user.Username = "batman";
            user.Password = "88ea39439e74fa27c09a4fc0bc8ebe6d00978392";
            ctx.Users.Add(user);
            ctx.SaveChanges();
        }
    }
}
