using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefullMethods;

namespace Classes
{
   public class Chat
    {
        public List<Message> Archive { get; set; }

        public Chat()
        {
            Archive = new List<Message>();
        }

    }
}
