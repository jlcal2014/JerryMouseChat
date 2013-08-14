using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JerryChat.Models
{
    public class Room
    {
        public Room()
        {
            this.Messages = new HashSet<Message>();
            this.Users = new HashSet<User>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MinLength(4)]
        [MaxLength(30)]
        public string Password { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
