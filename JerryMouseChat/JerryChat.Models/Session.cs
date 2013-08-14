using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JerryChat.Models
{
    public enum SessionStatus
    {
        Offline = 0,
        Online = 1,
        Away = 2
    }

    public class Session
    {
        [Key]
        public int Id { get; set; }
        public string SessionKey { get; set; }
        public SessionStatus Status { get; set; }
        public virtual User User { get; set; }
    }
}
