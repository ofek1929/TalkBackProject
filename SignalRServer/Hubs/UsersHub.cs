using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalRServer.BL;
using System.Threading.Tasks;
using CommonLibrary.Enums;
using CommonLibrary;

namespace SignalRServer.Hubs
{
    public class UsersHub : Hub
    {
        #region Members

        ChatManager _chatManager = new ChatManager();
        static Dictionary<string, string> _userConnections = new Dictionary<string, string>();
        static Dictionary<string, string> _userNames = new Dictionary<string, string>();

        #endregion

        #region Server Methods

        public IEnumerable<UserStatus> GetAllUsers()
        {
            return _chatManager.GetAllUsers();
        }

        public string Register(CommonUser user)
        {
            if (_userConnections.ContainsKey(Context.ConnectionId))
                return "Can't register when you already logged in";
            CheckForBuggedUsers();
            return _chatManager.Register(user, UserRegistered);
        }

        public string LogIn(CommonUser user)
        {
            if (_userConnections.ContainsKey(Context.ConnectionId))
                return "Can't log in when you already logged in";
            CheckForBuggedUsers();
            return _chatManager.LogIn(user, UserStatusChanged);
        }

        public string LogOff()
        {
            if (!_userConnections.ContainsKey(Context.ConnectionId))
                return "Can't log off when you not logged in";
            return _chatManager.LogOff(_userConnections[Context.ConnectionId], UserStatusChanged);
        }

        public string SendTo(string message, string toClientName)
        {
            if (!_userConnections.ContainsKey(Context.ConnectionId))
                return "Must log in first";
            if (!_userNames.ContainsKey(toClientName))
                return $"{toClientName} is not online";

            Clients.Client(_userNames[toClientName]).BroadcastMessage(_userConnections[Context.ConnectionId], message);
            return "";
        }
        public void SendGameTo(string userName, Game game)
        {
            try
            {
                if (!_userConnections.ContainsKey(Context.ConnectionId))
                    return;
                if (!_userNames.ContainsKey(userName))
                    return;
                //Clients.Client(_userNames[userName]).ReceiveGame(_userConnections[Context.ConnectionId], game );
                Clients.Client(_userNames[userName]).ReceiveGame(game);

            }
            catch (Exception e)
            {

                throw new Exception(e.ToString());
            }

        }

        public void SendGameMessageTo(string userName, string message)
        {
            if (!_userConnections.ContainsKey(Context.ConnectionId))
                return;
            if (!_userNames.ContainsKey(userName))
                return;
            //Clients.Client(_userNames[userName]).ReceiveGamemessage(_userConnections[Context.ConnectionId], message);
            Clients.Client(_userNames[userName]).ReceiveGamemessage(message);

        }


        public string SendToAll(string message)
        {
            if (!_userConnections.ContainsKey(Context.ConnectionId))
                return "Must log in first";

            Clients.All.BroadcastMessageAll(_userConnections[Context.ConnectionId], message);
            return "";
        }

        public string SendGameRequest(string userName)
        {
            if (!_userNames.ContainsKey(userName))
                return $"{userName} is not online";
            return _chatManager.SendGameRequest(userName, UserSentGameRequest);

        }

        public string AnswerGameRequest(bool answer, string userName)
        {
            if (!_userNames.ContainsKey(userName))
                return $"{userName} is not online";
            return _chatManager.AnswerGameRequest(answer, userName, _userConnections[Context.ConnectionId], UserAnsweredRequest, UserStatusChanged);
        }





        #endregion

        #region Private Methods

        private void CheckForBuggedUsers()
        {
            var users = GetAllUsers();
            var buggedUsers = users.Where(u => !_userNames.ContainsKey(u.Name) && (u.PlayerStatus == UserConnectionStatus.Online || u.PlayerStatus == UserConnectionStatus.InGame));
            foreach (var item in buggedUsers)
            {
                _chatManager.SetUserOffline(item.Name);
            }
        }
        #endregion

        #region BackToUser Methods

        private void UserRegistered(string userName)
        {
            _userConnections.Add(Context.ConnectionId, userName);
            _userNames.Add(userName, Context.ConnectionId);

            this.Clients.Others.RegistrationNotificated(userName);
        }

        private void UserStatusChanged(UserStatus userStatus)
        {

            if (userStatus.PlayerStatus == UserConnectionStatus.Online)
            {
                if (!_userConnections.ContainsKey(Context.ConnectionId))
                {
                    _userConnections.Add(Context.ConnectionId, userStatus.Name);
                    _userNames.Add(userStatus.Name, Context.ConnectionId);
                }
            }

            if (!_userConnections.ContainsKey(Context.ConnectionId)) return;

            var currentUser = _userConnections[Context.ConnectionId];
            if (userStatus.PlayerStatus == UserConnectionStatus.Offline)
            {
                _userConnections.Remove(Context.ConnectionId);
                _userNames.Remove(userStatus.Name);
            }
            if (userStatus.Name == currentUser)
                this.Clients.Others.UserStatusNotificated(userStatus);
            else
                this.Clients.All.UserStatusNotificated(userStatus);
        }

        private void UserSentGameRequest(string userName)
        {
            Clients.Client(_userNames[userName]).SentGameNotificated(_userConnections[Context.ConnectionId]);
        }

        private void UserAnsweredRequest(bool answer, string userName)
        {
            Clients.Client(_userNames[userName]).AnswerGameNotificated(answer, _userConnections[Context.ConnectionId]);
            if (answer)
            {
                var gameId = Guid.NewGuid().ToString();

                Clients.Clients(new List<string>() { _userNames[userName], Context.ConnectionId }).StartNewGameRoom(gameId);
            }
        }
        public void OnGameClosedOrEnd(string userName)
        {
            if (_userConnections.ContainsKey(Context.ConnectionId))
            {
                _chatManager.SetUserExitGame(userName, UserConnectionStatus.Online);

            }
        }
        public void OnEndGameMessageSend(string userName)
        {
            if (_userConnections.ContainsKey(Context.ConnectionId))
            {
                Clients.Client(_userNames[userName]).ReceiveEndGamemessage("the enemy exite the game, you can close the game");
                _chatManager.SetUserExitGame(userName, UserConnectionStatus.Online);
            }
        }


        public override Task OnDisconnected(bool stopCalled)
        {
            if (_userConnections.ContainsKey(Context.ConnectionId))
            {
                var userName = _userConnections[Context.ConnectionId];
                UserStatusChanged(new UserStatus(userName, UserConnectionStatus.Offline));
            }
            return base.OnDisconnected(stopCalled);
        }
        #endregion
    }
}