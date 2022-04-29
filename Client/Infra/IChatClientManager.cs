
using Client.Models;
using CommonLibrary;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Infra
{
    public interface IChatClientManager
    {
        Task<string> Register(CommonUser user);
        Task<string> LogIn(CommonUser user);
        string LogOff();
        Task<string> SendToAll(string message);
        Task<string> SendTo(string message, string userName);
        Task<string> SendGameRequest(string userName);
        Task<string> AnswerGameRequest(bool answer, string userName);
        Task<string> ExitGame(string gameId);

        Task<IEnumerable<UserStatus>> GetAllUsers();


        void RegisterToRegistrationEvent(Action<string> userNotificationMethod);
        void RegisterToUserStatusEvent(Action<UserStatus> userNotificationMethod);
        void RegisterToMessageRecievedEvent(Action<string, string> userNotificationMethod);
        void RegisterToSingleMsgRecievedEvent(Action<string, string> userNotificationMethod);
        void RegisterToGameRequestRecievedEvent(Action<string> userNotificationMethod);
        void RegisterToGameRequestAnswerRecievedEvent(Action<bool, string> userNotificationMethod);
        void RegisterToStartNewGameRoomEvent(Action userNotificationMethod);

        string UserName { get; }
        IHubProxy UserHubProxy { get ; }

    }
}
