using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class GameUsers
    {
        public string OwnUserName { get; set; }
        public string EnemyUserName { get; set; }

        public static GameUsers Instance = null;

        private GameUsers()
        {

        }
        public static GameUsers GetInstance()
        {
            if (Instance == null)
            {
                Instance = new GameUsers();
            }
            return Instance;
        }
    }
}
