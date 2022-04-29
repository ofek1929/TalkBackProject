using SignalRServer.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SignalRServer.DAL
{
    public class UsersContext : DbContext
    {
        public UsersContext() : base("UsersChatDB")
        {
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<UsersContext>());
        }
        public DbSet<User> Users { get; set; }
    }
}