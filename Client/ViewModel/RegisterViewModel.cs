using Client.Infra;
using CommonLibrary;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModel
{
    public class RegisterViewModel : ViewModelBase
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
                Register.RaiseCanExecuteChanged();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                Register.RaiseCanExecuteChanged();
            }
        }

        private bool _isRegistering;

        public bool IsRegistering
        {
            get { return _isRegistering; }
            set
            {
                _isRegistering = value;
                Register.RaiseCanExecuteChanged();
                NavigateToSignIn.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand Register { get; set; }
        public RelayCommand NavigateToSignIn { get; set; }

        #endregion

        public RegisterViewModel(IChatClientManager chatClientManager, IDialogService dialogService, IFrameNavigationService navigationService)
        {
            _chatClientManager = chatClientManager;
            _dialogService = dialogService;
            _navigationService = navigationService;

            InitCommands();
        }

        #region Methods

        private void InitCommands()
        {
            Register = new RelayCommand(async () =>
            {
                var user = new CommonUser(UserName, Password);
                IsRegistering = true;
                var errorMessage = await _chatClientManager.Register(user);
                IsRegistering = false;
                if (errorMessage != "")
                    _dialogService.ShowError(errorMessage, "Log in");
                else
                {
                    UserName = string.Empty;
                    Password = string.Empty;
                    _navigationService.NavigateTo("LobbyView");

                }
            }, () => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password) && !IsRegistering);

            NavigateToSignIn = new RelayCommand(() =>
            {
                UserName = string.Empty;
                Password = string.Empty;
                _navigationService.NavigateTo("SignInView");
            }, () => !IsRegistering);
        }

        #endregion
    }
}
