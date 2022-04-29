using CommonLibrary.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SignalRServer.DAL.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [NotMapped]
        public UserConnectionStatus PlayerStatus
        {
            get { return (UserConnectionStatus)Enum.Parse(typeof(UserConnectionStatus), Status); }
            set { Status = Enum.GetName(typeof(UserConnectionStatus), value); }
        }

        [Required]
        [Column("Player Status")]
        public string Status { get; private set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int Wins { get; set; }
    }
}