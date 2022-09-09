using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Classes
{
    public class LoginProp
    {
        public string i_Email { get; set; }
        public string i_Password { get; set; }

        public LoginProp(string i_Email, string i_Password)
        {
            this.i_Email = i_Email;
            this.i_Password = i_Password;
        }
    }
}