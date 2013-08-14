using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JerryChat.Models;

namespace JerryMouse.Repositories
{
    public class UsersRepository : EfRepository<User>
    {
        public UsersRepository(DbContext ctx)
            : base(ctx)
        {

        }
    }
}
