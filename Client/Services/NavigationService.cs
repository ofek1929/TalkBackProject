using Client.Views;
using GalaSoft.MvvmLight.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Services
{
    class NavigationService : INavigationService
    {
        public string CurrentPageKey => throw new NotImplementedException();

        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            Window window;
            if (pageKey == "Game")
            {
                window = new GameWindow(parameter);
            }
            else return;
            window.Show();
        }
    }
}
