using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Enums
{
    public enum UserConnectionStatus
    {
        Offline,
        Online,
        InGame
    }
    public enum PlayerCell
    {
        Empty,
        X,
        O
    }
    public enum MoveType
    {
        SelectSource,
        SelectDestination
    }
}
