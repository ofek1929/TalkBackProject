using Client.Infra;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Members

        IFrameNavigationService _navigationService;
        IChatClientManager _chatClientManager;

        #endregion

        #region Commands

        public RelayCommand LogOff { get; set; }
        private RelayCommand _loadedCommand;
        public RelayCommand LoadedCommand
        {
            get
            {
                return _loadedCommand
                    ?? (_loadedCommand = new RelayCommand(
                    () =>
                    {
                        _navigationService.NavigateTo("SignInView");
                    }));
            }
        }

        #endregion

        public MainViewModel(IFrameNavigationService navigationService, IChatClientManager chatClientManager)
        {
            _navigationService = navigationService;
            _chatClientManager = chatClientManager;
            LogOff = new RelayCommand(() =>
            {
                _chatClientManager.LogOff();
            });
        }
    }
}
