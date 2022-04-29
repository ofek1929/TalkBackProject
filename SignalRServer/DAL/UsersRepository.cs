using CommonLibrary.Enums;
using SignalRServer.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SignalRServer.DAL
{
    public class UsersRepository
    {
        UsersContext dbContext;

        public IEnumerable<User> GetAllUsers()
        {
            using (dbContext = new UsersContext())
            {
                return dbContext.Users.ToList();
            }
        }

        public void AddUser(User user)
        {
            if (user == null) throw new NullReferenceException();

            user.Password = SHA256Hash(user.Password);
            using (dbContext = new UsersContext())
            {
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
            }
        }

        public void UpdateDetails(User user)
        {
            if (user == null) throw new NullReferenceException();
          
            using (dbContext = new UsersContext())
            {
                var originalEntity = dbContext.Users.Find(user.ID);
                dbContext.Entry(originalEntity).CurrentValues.SetValues(user);
                dbContext.SaveChanges();
            }
        }

        public bool CheckLogin(string userName, string password)
        {
            password = SHA256Hash(password);
            using (dbContext = new UsersContext())
            {
                foreach (var user in dbContext.Users)
                {
                    if (user.Name == userName && user.Password == password)
                        return true;
                }
                return false;
            }
        }

        public User GetUserByUserName(string userName)
        {
            using (dbContext = new UsersContext())
            {
                foreach (var user in dbContext.Users)
                {
                    if (user.Name == userName)
                        return user;
                }
                return null;
            }
        }

        public void ChangeStatus(User user, UserConnectionStatus status)
        {
            if (user == null) throw new NullReferenceException();

            user.PlayerStatus = status;
            using (dbContext = new UsersContext())
            {
                var originalEntity = dbContext.Users.Find(user.ID);
                dbContext.Entry(originalEntity).CurrentValues.SetValues(user);
                dbContext.SaveChanges();
            }
        }

        public void Delete(User user)
        {
            if (user == null) throw new NullReferenceException();
            using (dbContext = new UsersContext())
            {
                var originalEntity = dbContext.Users.Find(user.ID);
                dbContext.Users.Remove(originalEntity);
                dbContext.SaveChanges();
            }
        }

        public void SetAllUsersToOffline()
        {
            using (dbContext = new UsersContext())
            {
                foreach (var item in dbContext.Users)
                {
                    item.PlayerStatus = UserConnectionStatus.Offline;
                }
                dbContext.SaveChanges();
            }
        }

        private static string SHA256Hash(string Data)
        {
            SHA256 sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(Data));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            return stringBuilder.ToString();
        }
    }
}