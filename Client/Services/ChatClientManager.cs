using Client.Infra;
using Client.Models;
using CommonLibrary;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Services
{
    public class ChatClientManager : IChatClientManager
    {
        #region Members

        static HubConnection _hubConnection;
        static IHubProxy _userHubProxy;
        static CommonUser _currentUser;

        #endregion

        #region Properties

        public IHubProxy UserHubProxy { get => _userHubProxy; }
        public string UserName { get => _currentUser?.Name; }

        #endregion


        static  ChatClientManager()
        {
            _hubConnection = new HubConnection(ConfigurationManager.AppSettings["HubConnectionPath"]);
            _userHubProxy = _hubConnection.CreateHubProxy("UsersHub");
            
        }

        #region Methods

        public async Task<string> Register(CommonUser user)
        {
            try
            {
                
                if (_hubConnection.State != ConnectionState.Connected)
                    await ConnectToServer();
                var registerTask = _userHubProxy.Invoke<string>("Register", user);
                _currentUser = user;
                return registerTask.Result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> LogIn(CommonUser user)
        {
            try
            {
                
                if (_hubConnection.State != ConnectionState.Connected)
                    await ConnectToServer();
                var logInTask = _userHubProxy.Invoke<string>("LogIn", user);
                _currentUser = user;
                return logInTask.Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
                return e.Message;
            }
        }

        public string LogOff()
        {
            try
            {
                var logOffTask = _userHubProxy.Invoke<string>("LogOff");
                logOffTask.Wait();
                _hubConnection.Closed -= _hubConnection_Closed;
                _hubConnection.Dispose();
                _currentUser = null;
                return logOffTask.Result;
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<IEnumerable<UserStatus>> GetAllUsers()
        {
            try
            {
                var getAllUsersTask = await _userHubProxy.Invoke<IEnumerable<UserStatus>>("GetAllUsers");
                return getAllUsersTask.OrderByDescending(u => u.PlayerStatus).ThenBy(u => u.Name);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<string> SendToAll(string message)
        {
            try
            {
                if (_hubConnection.State != ConnectionState.Connected)
                    await LogIn(_currentUser);
                await _userHubProxy.Invoke("SendToAll", message);
                return "";
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> SendTo(string message, string userName)
        {
            try
            {
                if (_hubConnection.State != ConnectionState.Connected)
                    await LogIn(_currentUser);
                var sendToTask = _userHubProxy.Invoke<string>("SendTo", message, userName);
                return sendToTask.Result;
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> SendGameRequest(string userName)
        {
            try
            {
                if (_hubConnection.State != ConnectionState.Connected)
                    await LogIn(_currentUser);
                var sendGameReqestTask = await _userHubProxy.Invoke<string>("SendGameRequest", userName);
                return sendGameReqestTask;
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> AnswerGameRequest(bool answer, string userName)
        {
            try
            {
                if (_hubConnection.State != ConnectionState.Connected)
                    await LogIn(_currentUser);
                var answerGameRequestTask = await _userHubProxy.Invoke<string>("AnswerGameRequest", answer, userName);

                return answerGameRequestTask;
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> ExitGame(string gameId)
        {
            try
            {
                if (_hubConnection.State != ConnectionState.Connected)
                    await LogIn(_currentUser);
                var exitGameTask = await _userHubProxy.Invoke<string>("ExitGame", gameId);
                return exitGameTask;
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }

        #endregion

        #region Connection Methods

        private async Task<bool> ConnectToServer()
        {
            try
            {
                Console.WriteLine(_hubConnection.State.ToString());
                await _hubConnection.Start();
                Console.WriteLine(_hubConnection.State.ToString());
                _hubConnection.Closed += _hubConnection_Closed;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async void _hubConnection_Closed()
        {
            //TimeSpan retryDuration = TimeSpan.FromMinutes(1);
            TimeSpan retryDuration = TimeSpan.FromSeconds(30);
            DateTime retryTill = DateTime.Now.Add(retryDuration);
            while (DateTime.Now < retryTill)
            {
                bool connected = await ConnectToServer();
                if (connected)
                {
                    await LogIn(_currentUser);
                    return;
                }
            }
        }

        #endregion

        #region Events

        public void RegisterToRegistrationEvent(Action<string> userNotificationMethod)
        {
            _userHubProxy.On("RegistrationNotificated", (string userName) =>
            {
                Application.Current.Dispatcher.Invoke(() => userNotificationMethod.Invoke(userName));
            });
        }

        public void RegisterToUserStatusEvent(Action<UserStatus> userNotificationMethod)
        {
            _userHubProxy.On("UserStatusNotificated", (UserStatus userStatus) =>
            {
                Application.Current.Dispatcher.Invoke(() => userNotificationMethod.Invoke(userStatus));
            });
        }

        public void RegisterToMessageRecievedEvent(Action<string, string> userNotificationMethod)
        {
            _userHubProxy.On("BroadcastMessageAll", (string userName, string message) =>
            {
                Application.Current.Dispatcher.Invoke(() => userNotificationMethod.Invoke(userName, message));
            });
        }

        public void RegisterToSingleMsgRecievedEvent(Action<string, string> userNotificationMethod)
        {
            _userHubProxy.On("BroadcastMessage", (string userName, string message) =>
            {
                Application.Current.Dispatcher.Invoke(() => userNotificationMethod.Invoke(userName, message));
            });
        }

        public void RegisterToGameRequestRecievedEvent(Action<string> userNotificationMethod)
        {
            _userHubProxy.On("SentGameNotificated", (string userName) =>
            {
                Application.Current.Dispatcher.Invoke(() => userNotificationMethod.Invoke(userName));
            });
        }

        public void RegisterToGameRequestAnswerRecievedEvent(Action<bool, string> userNotificationMethod)
        {
            _userHubProxy.On("AnswerGameNotificated", (bool answer, string userName) =>
            {
                Application.Current.Dispatcher.Invoke(() => userNotificationMethod.Invoke(answer, userName));
            });
        }

        public void RegisterToStartNewGameRoomEvent(Action userNotificationMethod)
        {
            _userHubProxy.On("StartNewGameRoom", () =>
            {
                Application.Current.Dispatcher.Invoke(() => userNotificationMethod.Invoke());
            });
           
        }
        #endregion
    }
}
