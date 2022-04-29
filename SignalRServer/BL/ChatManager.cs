using CommonLibrary;
using CommonLibrary.Enums;
using SignalRServer.DAL;
using SignalRServer.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SignalRServer.BL
{
    public class ChatManager
    {
        UsersRepository _usersRepository = new UsersRepository();

        public IEnumerable<UserStatus> GetAllUsers()
        {
            List<UserStatus> allUsers = new List<UserStatus>();

            foreach (User user in _usersRepository.GetAllUsers())
            {
                var userStatus = new UserStatus(user.Name, user.PlayerStatus, user.Wins);
                allUsers.Add(userStatus);
            }
            return allUsers;
        }

        public string Register(CommonUser user, Action<string> userNotificationMethod)
        {
            if (user == null) return "User is null";
            if (string.IsNullOrWhiteSpace(user.Name))
                return "Can't choose an empty name";
            if (user.Name.Length > 10)
                return "Can't choose a name with more than 10 letters";
            if (user.Name.Contains(" "))
                return "Can't choose a name with spaces";
            if (_usersRepository.GetAllUsers().Any((u) => u.Name == user.Name))
                return "User already exists";
            if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Count() < 5)
                return "Password length must be 5 or more";


            _usersRepository.AddUser(new User() { Name = user.Name, Password = user.Password, PlayerStatus = UserConnectionStatus.Online });
            Task.Run(() =>
            {
                userNotificationMethod(user.Name);
            });

            return "";

        }
        public void SetUserExitGame (string userName, UserConnectionStatus status)
        {
            User user = _usersRepository.GetUserByUserName(userName);
            _usersRepository.ChangeStatus(user, status);
        }
        public string LogIn(CommonUser user, Action<UserStatus> userNotificationMethod)
        {
            if (user == null) return "User is null";
            if (!_usersRepository.GetAllUsers().Any((u) => u.Name == user.Name))
                return "User doesn't exists";
            if (string.IsNullOrWhiteSpace(user.Password) || !_usersRepository.CheckLogin(user.Name, user.Password))
                return "Incorrect password";

            var originalUser = _usersRepository.GetUserByUserName(user.Name);
            if (originalUser.PlayerStatus == UserConnectionStatus.Online || originalUser.PlayerStatus == UserConnectionStatus.InGame)
                return "Username already in use";

            _usersRepository.ChangeStatus(originalUser, UserConnectionStatus.Online);
            Task.Run(() =>
            {
                userNotificationMethod(new UserStatus(user.Name, UserConnectionStatus.Online));
            });

            return "";

        }

        public string LogOff(string userName, Action<UserStatus> userNotificationMethod)
        {
            if (userName == null) return "User is null";
            if (!_usersRepository.GetAllUsers().Any((u) => u.Name == userName))
                return "User doesn't exists";
          
            SetUserOffline(userName);
            Task.Run(() =>
            {
                userNotificationMethod(new UserStatus(userName, UserConnectionStatus.Offline));
            });

            return "";

        }

        public string SendGameRequest(string userName, Action<string> userNotificationMethod)
        {
            if (userName == null) return "User is null";

            //var originalUser = _usersRepository.GetUserByUserName(userName);                         //commented because now you can play multiple games at the same time
            //if (originalUser.PlayerStatus == PlayerStatus.InGame)                                    
            //    return $"{userName} is already in a game";                                          

            Task.Run(() =>
            {
                userNotificationMethod(userName);
            });
            return "";
        }

        public string AnswerGameRequest(bool answer, string userName, string fromUserName, Action<bool, string> userNotificationMethod, Action<UserStatus> userStatusChangeMethod)
        {
            if (userName == null) return "User is null";

            var originalUser = _usersRepository.GetUserByUserName(userName);
            //if (originalUser.PlayerStatus == PlayerStatus.InGame)                                      //commented because now you can play multiple games at the same time
            //    return $"{userName} is already in a game";
            if (answer)
            {
                _usersRepository.ChangeStatus(originalUser, UserConnectionStatus.InGame);
                _usersRepository.ChangeStatus(_usersRepository.GetUserByUserName(fromUserName), UserConnectionStatus.InGame);
            }
            Task.Run(() =>
            {
                userNotificationMethod(answer, userName);
                if (answer)
                {
                    userStatusChangeMethod(new UserStatus(userName, UserConnectionStatus.InGame));
                    userStatusChangeMethod(new UserStatus(fromUserName, UserConnectionStatus.InGame));
                }
            });
            return "";
        }

        public void SetUserOffline(string userName)
        {
            _usersRepository.ChangeStatus(_usersRepository.GetUserByUserName(userName), UserConnectionStatus.Offline);
        }
    }
}