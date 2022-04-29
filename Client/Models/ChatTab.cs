using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class ChatTab : INotifyPropertyChanged
    {
        public string UserNameTab { get; set; }
        public ObservableCollection<ChatMessageDetails> Chat { get; set; }

        private int _unreadMessages;
        public int UnreadMessages
        {
            get { return _unreadMessages; }
            set
            {
                _unreadMessages = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnreadMessages)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
