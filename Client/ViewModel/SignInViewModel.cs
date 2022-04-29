using Client.Infra;
using CommonLibrary;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.ViewModel
{
    public class SignInViewModel : ViewModelBase
    {
        #region Members

        IChatClientManager _chatClientManager;
        IDialogService _dialogService;
        IFrameNavigationService _navigationService;

        #endregion

        #region Properties
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                LogIn.RaiseCanExecuteChanged();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                LogIn.RaiseCanExecuteChanged();
            }
        }

        private bool _isSigningIn;
        public bool IsSigningIn
        {
            get { return _isSigningIn; }
            set
            {
                _isSigningIn = value;
                LogIn.RaiseCanExecuteChanged();
                NavigateToRegister.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand LogIn { get; set; }
        public RelayCommand NavigateToRegister { get; set; }

        #endregion

        public SignInViewModel(IChatClientManager chatClientManager, IDialogService dialogService, IFrameNavigationService navigationService)
        {
            _chatClientManager = chatClientManager;
            _dialogService = dialogService;
            _navigationService = navigationService;

            InitCommands();
        }

        #region Methods

        private void InitCommands()
        {
            LogIn = new RelayCommand(async () =>
             {
                 var user = new CommonUser(UserName, Password);
                 IsSigningIn = true;
                 var errorMessage = await _chatClientManager.LogIn(user);
                 IsSigningIn = false;
                 if (errorMessage != "")
                     _dialogService.ShowError(errorMessage, "Log in");
                 else
                 {
                     UserName = string.Empty;
                     Password = string.Empty;
                     _navigationService.NavigateTo("LobbyView");
                 }
             }, () => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !IsSigningIn);

            NavigateToRegister = new RelayCommand(() =>
            {
                UserName = string.Empty;
                Password = string.Empty;
                _navigationService.NavigateTo("RegisterView");
            }, () => !IsSigningIn);
        }

        #endregion
    }
}
