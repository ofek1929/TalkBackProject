using CommonLibrary.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class UserStatus : INotifyPropertyChanged
    {
        public string Name { get; set; }

        private UserConnectionStatus _playerStatus;
        public UserConnectionStatus PlayerStatus
        {
            get { return _playerStatus; }
            set
            {
                _playerStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PlayerStatus)));
            }
        }

        private int _wins;
        public int Wins
        {
            get { return _wins; }
            set
            {
                _wins = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wins)));
            }
        }


        public UserStatus(string name, UserConnectionStatus playerStatus, int wins = 0)
        {
            Name = name;
            PlayerStatus = playerStatus;
            Wins = wins;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
