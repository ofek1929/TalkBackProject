
using CommonLibrary;
using CommonLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class Player
    {
        public string UserName { get; set; }

        public bool IsPlay { get; set; }

        public PlayerCell PlayerColor { get; set; }
    }
    
}
