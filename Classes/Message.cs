using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsefullMethods;

namespace Classes
{
    public class Message
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Body { get; set; }

        public Message()
        {
            
        }

        public Message(string i_UserName, string i_Body)
        {
            Id = "message_" + SystemTools.RandomString(20); ;
            UserName = i_UserName;
            Body = i_Body;
        }
    }

   
}
