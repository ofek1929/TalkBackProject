using CommonLibrary;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Services
{
    public class GameClientManager
    {
        public string UserName { get; private set; }
        public IHubProxy HubProxy { get; private set; }
        public GameClientManager(string userName, IHubProxy hubProxy)
        {
            UserName = userName;
            HubProxy = hubProxy;
        }

        #region Methods
        
        public async void SendGame(Game game, string userName)
        {

            await HubProxy.Invoke("SendGameTo", userName, game);
        }

        public async void SendGameMessage(string message, string userName)
        {
           await HubProxy.Invoke("SendGameMessageTo", userName, message);

        }
        #endregion

        public async void GameEnd(string username)
        {
            await HubProxy.Invoke("OnGameClosedOrEnd", username);       

        }   
        public async void SendEndGameMessage(string username)
        {
            await HubProxy.Invoke("OnEndGameMessageSend", username);       

        }


        #region Events
        public void RegisterToReceiveEndGameMessage(Action<string> userNotificationMethod)
        {
            HubProxy.On("ReceiveEndGamemessage", (string message) =>
            {
                Application.Current.Dispatcher.Invoke(() => userNotificationMethod.Invoke(message));
            });
        }
        public void RegisterToReceiveGameMessage(Action<string> userNotificationMethod)
        {
            HubProxy.On("ReceiveGamemessage", (string message) =>
            {
                Application.Current.Dispatcher.Invoke(() => userNotificationMethod.Invoke(message));
            });
        }
        public void RegisterToReceiveGame(Action<Game> userNotificationMethod)
        {
            HubProxy.On("ReceiveGame", (Game game) =>
            {
                Application.Current.Dispatcher.Invoke(() => userNotificationMethod.Invoke(game));
            });
        }

        #endregion
    }
}
