using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JerryChat.Models
{
    public class User
    {
        public User()
        {
            this.Messages = new HashSet<Message>();
            this.Rooms = new HashSet<Room>();
        }

        [Key]
        public int Id { get; set; }
        
        [Required]
        [MinLength(6)]
        [MaxLength(30)]
        public string Username { get; set; }

        [MinLength(40)]
        [MaxLength(40)]
        [Required]
        public string Password { get; set; }

        public string AvatarUrl { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
