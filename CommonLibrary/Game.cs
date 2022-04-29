
using CommonLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Media.Imaging;

namespace CommonLibrary
{
    public class Game
    {
        public Player X { get; set; } //the palyer user data
        public Player O { get; set; }//the palyer user data
        public PlayerCell WinnerPlayer { get; set; }
        public Board Board { get; set; }
        public PlayerCell CurrentTrun { get; set; }
        public MoveType MoveType { get; set; }
        public int LastSourceIndex { get; set; }


        public Game()
        {
            WinnerPlayer = PlayerCell.Empty;
            Board = new Board();
            X = new Player();
            O = new Player();
            X.PlayerColor = PlayerCell.X;
            O.PlayerColor = PlayerCell.O;
        }
        public string ReturnImageSourceToCurrntPlayer()
        {
            string source;
            if (CurrentTrun == PlayerCell.O)
            {
                //source = "ms-appx:///Assets/o.png";
                //source = "//Assets/o.png";
                source = "C:/Users/ofek/Desktop/1030 לימודים סלע/BackgammonAndChatProject4/TalkBackProject/Client/Assets/o.png";

            }
            else
            {
                //source = "ms-appx:///Assets/x.png";
                source = "C:/Users/ofek/Desktop/1030 לימודים סלע/BackgammonAndChatProject4/TalkBackProject/Client/Assets/x.png";
            }
            return source;
            //return new BitmapImage(new Uri(source));
        }

        public string ReturnImageSourceToCellColor(PlayerCell player)
        {
            string source = "";
            if (player == PlayerCell.X)
            {
                source = "C:/Users/ofek/Desktop/1030 לימודים סלע/BackgammonAndChatProject4/TalkBackProject/Client/Assets/x.png";
            }
            else if (player == PlayerCell.O)
            {
                source = "C:/Users/ofek/Desktop/1030 לימודים סלע/BackgammonAndChatProject4/TalkBackProject/Client/Assets/o.png";
            }
            return source;
        }
    }
}
