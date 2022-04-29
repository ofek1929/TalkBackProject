using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Client.Infra;
using Client.Models;
using CommonLibrary;
using CommonLibrary.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace Client.ViewModel
{

    public class LobbyViewModel : ViewModelBase
    {
        #region Members

        IChatClientManager _chatClientManager;
        Infra.IDialogService _dialogService;
        IFrameNavigationService _navigationService;
        INavigationService _windowNavigationService;
        Dictionary<string, SolidColorBrush> _usersColors;
        Random rnd = new Random();

        #endregion

        #region Properties

        public ObservableCollection<UserStatus> UsersList { get; set; }
        public ObservableCollection<ChatTab> TabsList { get; set; }
        public string UserName { get => _chatClientManager.UserName; }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                SendMessage.RaiseCanExecuteChanged();
            }
        }

        private int _userWins;
        public int UserWins
        {
            get { return _userWins; }
            set
            {
                _userWins = value;
                RaisePropertyChanged();
            }
        }

        public int SelectedTab { get; set; }
        public UserStatus SelectedUser { get; set; }

        #endregion

        #region Commands

        public RelayCommand LogOff { get; set; }
        public RelayCommand GetUsersList { get; set; }
        public RelayCommand InitChat { get; set; }
        public RelayCommand SendMessage { get; set; }
        public RelayCommand OpenChatTab { get; set; }
        public RelayCommand<string> CloseTab { get; set; }
        public RelayCommand ResetUnreadMsgCount { get; set; }
        public RelayCommand SendGameRequest { get; set; }

        #endregion

        public LobbyViewModel(IChatClientManager chatClientManager, Infra.IDialogService dialogService, IFrameNavigationService navigationService, INavigationService windowNavigationService)
        {
            _chatClientManager = chatClientManager;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _windowNavigationService = windowNavigationService;
            InitCommands();
            RegisterToEvents();
        }

        #region Methods

        private void InitCommands()
        {
            LogOff = new RelayCommand(() =>
            {
                _chatClientManager.LogOff();
                _navigationService.NavigateTo("SignInView");
            });

            GetUsersList = new RelayCommand(async () =>
             {
                 UsersList = new ObservableCollection<UserStatus>(await _chatClientManager.GetAllUsers());
                 _usersColors = new Dictionary<string, SolidColorBrush>();
                 foreach (var item in UsersList)
                 {
                     _usersColors.Add(item.Name, GetRandomColor());
                 }
                 var user = UsersList.First((u) => u.Name == UserName);
                 UserWins = user.Wins;
                 UsersList.Remove(user);
                 RaisePropertyChanged(() => UsersList);
                 SelectedTab = 0;
                 RaisePropertyChanged(() => SelectedTab);
             });

            InitChat = new RelayCommand(() =>
            {
                TabsList = new ObservableCollection<ChatTab>()
                {
                    new ChatTab() { UserNameTab = "Lobby", Chat= new ObservableCollection<ChatMessageDetails>() }
                };
                RaisePropertyChanged(() => TabsList);
            });

            SendMessage = new RelayCommand(async () =>
            {
                string errorMessage;

                if (SelectedTab == 0 || SelectedTab == -1)
                    errorMessage = await _chatClientManager.SendToAll(Message);
                else
                {
                    errorMessage = await _chatClientManager.SendTo(Message, TabsList[SelectedTab].UserNameTab);
                    TabsList[SelectedTab].Chat.Add(new ChatMessageDetails()
                    {
                        Text = Message,
                        IsYou = true,
                        Color = _usersColors[UserName],
                        Time = DateTime.Now.ToShortTimeString()
                    });
                }

                if (errorMessage != "")
                    _dialogService.ShowError(errorMessage, "While sending message");
                Message = string.Empty;
                RaisePropertyChanged(() => Message);
            }, () => !string.IsNullOrWhiteSpace(Message));

            OpenChatTab = new RelayCommand(() =>
             {
                 if (!TabsList.Any(c => c.UserNameTab == SelectedUser.Name))
                     TabsList.Add(new ChatTab() { UserNameTab = SelectedUser.Name, Chat = new ObservableCollection<ChatMessageDetails>() });
                 SelectedTab = TabsList.IndexOf(TabsList.FirstOrDefault(c => c.UserNameTab == SelectedUser.Name));
                 RaisePropertyChanged(() => SelectedTab);
             });

            CloseTab = new RelayCommand<string>((userName) =>
            {
                TabsList.Remove(TabsList.FirstOrDefault(c => c.UserNameTab == userName));
            });

            ResetUnreadMsgCount = new RelayCommand(() =>
            {
                if (SelectedTab == -1) return;
                TabsList[SelectedTab].UnreadMessages = 0;
            });

            SendGameRequest = new RelayCommand(async () =>
            {
                var errorMessage = await _chatClientManager.SendGameRequest(SelectedUser.Name);
                if (errorMessage != "")
                    _dialogService.ShowError(errorMessage, "Game Request");
            });
        }

        private void RegisterToEvents()
        {
            _chatClientManager.RegisterToRegistrationEvent(OnRegistrationNotificated);
            _chatClientManager.RegisterToUserStatusEvent(OnUserStatusNotificated);
            _chatClientManager.RegisterToMessageRecievedEvent(OnMessageRecievedNotificated);
            _chatClientManager.RegisterToSingleMsgRecievedEvent(OnPrivateMsgRecievedNotificated);
            _chatClientManager.RegisterToGameRequestRecievedEvent(OnGameRequestRecievedNotificated);
            _chatClientManager.RegisterToGameRequestAnswerRecievedEvent(OnGameRequestAnswerRecievedNotificated);
            _chatClientManager.RegisterToStartNewGameRoomEvent(OnStartNewGameRoomNotificated);
        }

        private SolidColorBrush GetRandomColor()
        {
            var color = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 180), (byte)rnd.Next(1, 180), (byte)rnd.Next(1, 180)));
            return color;
        }

        #endregion

        #region Events

        private void OnRegistrationNotificated(string userName)
        {
            if (UsersList == null) return;
            UsersList.Add(new UserStatus(userName, UserConnectionStatus.Online));
            _usersColors.Add(userName, GetRandomColor());
            UsersList = new ObservableCollection<UserStatus>(UsersList.OrderByDescending(u => u.PlayerStatus).ThenBy(u => u.Name));
            RaisePropertyChanged(() => UsersList);
        }

        private void OnUserStatusNotificated(UserStatus userStatus)
        {
            if (UsersList == null) return;
            if (userStatus.Name == _chatClientManager.UserName)
            {
                if (userStatus.Wins != 0)
                    UserWins = userStatus.Wins;
                return;
            }

            var user = UsersList.First((u) => u.Name == userStatus.Name);
            user.PlayerStatus = userStatus.PlayerStatus;
            if (userStatus.Wins != 0)
                user.Wins = userStatus.Wins;
            UsersList = new ObservableCollection<UserStatus>(UsersList.OrderByDescending(u => u.PlayerStatus).ThenBy(u => u.Name));
            RaisePropertyChanged(() => UsersList);
        }

        private void OnMessageRecievedNotificated(string userName, string message)
        {
            TabsList[0].Chat.Add(new ChatMessageDetails()
            {
                Text = $"{userName} : {message}",
                IsYou = userName == UserName,
                Color = _usersColors[userName],
                Time = DateTime.Now.ToShortTimeString()
            });
        }

        private void OnPrivateMsgRecievedNotificated(string userName, string message)
        {
            if (!TabsList.Any(c => c.UserNameTab == userName))
                TabsList.Add(new ChatTab() { UserNameTab = userName, Chat = new ObservableCollection<ChatMessageDetails>() });
            var tabNumber = TabsList.IndexOf(TabsList.FirstOrDefault(c => c.UserNameTab == userName));
            TabsList[tabNumber].Chat.Add(new ChatMessageDetails()
            {
                Text = message,
                IsYou = false,
                Color = _usersColors[userName],
                Time = DateTime.Now.ToShortTimeString()
            });
            if (tabNumber != SelectedTab)
                TabsList[tabNumber].UnreadMessages++;

        }
        private async void OnGameRequestRecievedNotificated(string userName)
        {
            var result = _dialogService.ShowQuestion($"{userName} sent a game reqest", "Game Request");
            if (result == true)
            {
                GameUsers.GetInstance().EnemyUserName = UserName;
                GameUsers.GetInstance().OwnUserName = userName;

            }
            var errorMessage = await _chatClientManager.AnswerGameRequest(result, userName);
            if (errorMessage != "")
                _dialogService.ShowError(errorMessage, "Game Request");

        }

        private void OnGameRequestAnswerRecievedNotificated(bool answer, string userName)
        {
            if (!answer)
                _dialogService.ShowMessage($"{userName} rejected your game offer", "Game Request");
            else
            {
                GameUsers.GetInstance().EnemyUserName = userName;
                GameUsers.GetInstance().OwnUserName = UserName;
            }
        }

        private void OnStartNewGameRoomNotificated()
        {            
            _windowNavigationService.NavigateTo("Game");
        }

        #endregion

    }
}