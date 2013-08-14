using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JerryChat.Services.Models
{
    public class MessageModel
    {
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
    }
}