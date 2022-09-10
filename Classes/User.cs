using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UsefullMethods;
using System.Data.Entity;

namespace Classes
{
   public class User
    {
        [Required]
        public string Name { get; set; }  //need to change the accessability level
        [Required]
        [Key]
        public string Email { get; set; } //need to change the accessability level
        [Required]
        public string Password { get; set; } //need to change the accessability level

        public int? Figure { get; set; }

        public User()
        {
            
        }

        public User(string i_Name, string i_Email, string i_Password)
        {
            
            Name = i_Name;
            Email = i_Email;
            Password = i_Password;
            Figure = 0;
        }

        public User(string i_Name, string i_Email, int i_Skin)
        {

            Name = i_Name;
            Email = i_Email;
            Figure = i_Skin;
        }

    }
}
