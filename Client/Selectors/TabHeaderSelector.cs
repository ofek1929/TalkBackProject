using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace Client.Selectors
{
    public class TabHeaderSelector: DataTemplateSelector
    {
        public DataTemplate LobbyHeaderTemplate { get; set; }
        public DataTemplate UserHeaderTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var tab = (ChatTab)item;
            if (tab.UserNameTab == "Lobby")
                return LobbyHeaderTemplate;
            return UserHeaderTemplate;
        }
    }
}
