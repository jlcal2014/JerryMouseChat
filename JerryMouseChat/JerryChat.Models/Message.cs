using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JerryChat.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public virtual User Author { get; set; }

        [Required]
        public virtual Room Room { get; set; }
    }
}
