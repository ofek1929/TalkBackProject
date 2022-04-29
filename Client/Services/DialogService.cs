
using Client.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Services
{
    public class DialogService : IDialogService
    {
        public void ShowError(Exception Error, string Title)
        {
            MessageBox.Show(Error.ToString(), Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowError(string Message, string Title)
        {
            MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInfo(string Message, string Title)
        {
            MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowMessage(string Message, string Title)
        {
            MessageBox.Show(Message, Title, MessageBoxButton.OK);
        }

        public bool ShowQuestion(string Message, string Title)
        {
            return MessageBox.Show(new Window() {Topmost=true}, Message, Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public void ShowWarning(string Message, string Title)
        {
            MessageBox.Show(Message, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
