using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class CommonUser
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public CommonUser(string name,string password)
        {
            Name = name;
            Password = password;
        }
    }
}
