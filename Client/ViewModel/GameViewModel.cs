
using Client.Extentions;
using Client.Infra;
using Client.Models;
using Client.Services;
using Client.Views;
using CommonLibrary;
using CommonLibrary.Enums;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        private IChatClientManager _chatClientManager;
        private IDialogService _dialogService;
        private GameClientManager _gameClientManager;
        private Timer timer;
        private Rectangle[,] _rectangles;
        private TextBox _tbxMessage;
        private Button _btnSendMessage;
        private Button _btnExitGame;
        private ICommand sendMessageComand;
        private ICommand _exitGameCommand;
        
        private ListView _privateChat;
        public string UserName { get; set; }
        public ObservableCollection<ChatMessageDetails> Chat { get; private set; }
        public Grid GameGrid { get; set; }
        public Grid ChatGrid { get; set; }
        public Grid Grid { get; set; }
        public Game board { get; set; }

        public Image imageX { get; private set; }
        public Image imageO { get; private set; }
        //private bool _isWin = false;

        public GameViewModel(IChatClientManager chatClientManager, IDialogService dialogService, IFrameNavigationService navigationService)
        {
            sendMessageComand = new RelayCommand(sendMessage);
            _exitGameCommand = new RelayCommand(ExitGame);
            _chatClientManager = chatClientManager;
            _dialogService = dialogService;
            _gameClientManager = new GameClientManager(_chatClientManager.UserName, _chatClientManager.UserHubProxy);//in servises
            UserName = _chatClientManager.UserName;
            Chat = new ObservableCollection<ChatMessageDetails>();//in models
            _rectangles = new Rectangle[3, 3];
            RegisterToEvents();
            CreatGame();
            if ((board.O.UserName == UserName && board.CurrentTrun == board.O.PlayerColor)|| (board.X.UserName == UserName && board.CurrentTrun == board.X.PlayerColor))
            {
                StartMouseDownEvent();
            }
        }

        private void ExitGame()
        {
            string Enemyusername;
            if (board.X.UserName == UserName)
            {
                Enemyusername = board.O.UserName;
            }
            else
            {
                Enemyusername = board.X.UserName;
            }
            _gameClientManager.GameEnd(UserName);
            _gameClientManager.SendEndGameMessage(Enemyusername);
            MessageBox.Show("The game is over you can now close the game");

        }

        private void sendMessage()
        {
            string username;
            string message = _tbxMessage.Text;
            _privateChat.Items.Add($" {UserName} said {_tbxMessage.Text}");
            _tbxMessage.Text = "";
            if (board.X.UserName == UserName)
            {
                username = board.O.UserName;
            }
            else
            {
                username = board.X.UserName;            
            }
            _gameClientManager.SendGameMessage(message, username);
        }

        private void StartMouseDownEvent()//Set MouseDown event for the rectangle element in the grid
        {

            for (int i = 0; i < board.Board.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < board.Board.Cells.GetLength(1); j++)
                {
                    if (board.Board.Cells[i, j].CellColor == PlayerCell.Empty)
                    {
                        //board.Board.Cells[i, j].Rectangle.StrokeThickness = 5;
                        //board.Board.Cells[i, j].Rectangle.MouseDown += Rectangle_MouseDown; 
                        _rectangles[i,j].StrokeThickness = 5;
                        _rectangles[i, j].MouseDown += Rectangle_MouseDown;
                    }
                    else
                    {
                        GameGrid.SetElementToGrid(new Image { Source = new BitmapImage(new Uri(board.ReturnImageSourceToCellColor(board.Board.Cells[i, j].CellColor))) }, i, j);
                    }
                }
            }
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CloseAllEvents();
            Rectangle r = sender as Rectangle;
            int col = Grid.GetColumn(r);
            int row = Grid.GetRow(r);
            r.StrokeThickness = 0;
            //board.Board.Cells[col, row].CellColor = board.CurrentTrun;
            board.Board.Cells[ row,col].CellColor = board.CurrentTrun;
            GameGrid.SetElementToGrid(new Image { Source = new BitmapImage(new Uri(board.ReturnImageSourceToCurrntPlayer())) }, row, col);
            CheckIfWin();
            board.X.IsPlay = board.CurrentTrun == board.X.PlayerColor ? true : false;
            board.O.IsPlay = board.CurrentTrun == board.O.PlayerColor ? true : false;       
            board.CurrentTrun = board.CurrentTrun == PlayerCell.O ? PlayerCell.X : PlayerCell.O;
            StartTimer();

        }
        private void StartTimer()
        {
            timer = new Timer();
            timer.Interval = 700;
            timer.Elapsed += Timer_Elapsed; ;
            timer.Start();

        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //CloseAllEvents();
            SendGameToEnemy();
            timer.Stop();
        }

        private void SendGameToEnemy()
        {
            if (board.X.PlayerColor == board.CurrentTrun) _gameClientManager.SendGame(board, board.X.UserName);
            else _gameClientManager.SendGame(board, board.O.UserName);
        }

        private  void CloseAllEvents()
        {
            for (int i = 0; i < board.Board.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < board.Board.Cells.GetLength(1); j++)
                {
                   _rectangles[i,j].MouseDown -= Rectangle_MouseDown;
                   _rectangles[i,j].StrokeThickness = 0;
                }
            }
        }   


        private void CheckIfWin()
        {
            //check on row
            for (int i = 0; i < board.Board.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < board.Board.Cells.GetLength(1); j++)
                {
                    if (board.Board.Cells[i, j].CellColor == board.CurrentTrun)
                    {
                        if (j == board.Board.Cells.GetLength(1) - 1)
                        {
                            //win
                            board.WinnerPlayer = board.CurrentTrun;
                            ShowWinMessage(board.CurrentTrun);
                            _gameClientManager.GameEnd(UserName);
                            MessageBox.Show("The game is over you can now close the game");
                            //_isWin = true;
                        }
                    }
                    else break;
                }
            }
            //check on colmn
            for (int i = 0; i < board.Board.Cells.GetLength(1); i++)
            {
                for (int j = 0; j < board.Board.Cells.GetLength(0); j++)
                {
                    if (board.Board.Cells[j, i].CellColor == board.CurrentTrun)
                    {
                        if (j == board.Board.Cells.GetLength(0) - 1)
                        {
                            //win
                            board.WinnerPlayer = board.CurrentTrun;
                            ShowWinMessage(board.CurrentTrun);
                            _gameClientManager.GameEnd(UserName);
                            MessageBox.Show("The game is over you can now close the game");
                            //_isWin = true;
                        }
                    }
                    else break;
                }
            }
            //alcson \
            for (int i = 0, j = 0; i < board.Board.Cells.GetLength(0) || j < board.Board.Cells.GetLength(1); i++, j++)
            {
                if (board.Board.Cells[i, j].CellColor == board.CurrentTrun)
                {
                    if (i == 2 && j == 2)
                    {
                        //win
                        board.WinnerPlayer = board.CurrentTrun;
                        ShowWinMessage(board.CurrentTrun);
                        _gameClientManager.GameEnd(UserName);
                        MessageBox.Show("The game is over you can now close the game");
                        //_isWin = true;

                    }
                }
                else break;
            }
            //slcson /
            for (int i = 0, j = 2; i < board.Board.Cells.GetLength(0) || j >= 0; i++, j--)
            {
                if (board.Board.Cells[i, j].CellColor == board.CurrentTrun)
                {
                    if (i == 2 && j == 0)
                    {
                        //win
                        board.WinnerPlayer = board.CurrentTrun;
                        ShowWinMessage(board.CurrentTrun);
                        _gameClientManager.GameEnd(UserName);
                        MessageBox.Show("The game is over you can now close the game");
                        //_isWin = true;
                    }
                }
                else break;
            }
        }

        private void ShowWinMessage(PlayerCell currentTrun)
        {
            if (currentTrun == PlayerCell.O)
            {
                MessageBox.Show(" O Win ", "Winner");
            }
            else
            {
                MessageBox.Show(" X Win ", "Winner");
            }
        }

        private void CreatGame()
        {



            #region SetGrid 


            Grid = new Grid();
            Grid.ShowGridLines = true;
            Grid.ColumnDefinitions.Add(new ColumnDefinition ()) ;
            Grid.ColumnDefinitions.Add(new ColumnDefinition ());

            board = new Game();
            board.O.UserName = GameUsers.GetInstance().OwnUserName;
            board.CurrentTrun = PlayerCell.O;
            board.X.UserName = GameUsers.GetInstance().EnemyUserName;
            GameGrid = new Grid { ShowGridLines = true };
            ChatGrid = new Grid { ShowGridLines = true };
            Grid.SetElementToGrid(GameGrid,0,0);
            Grid.SetElementToGrid(ChatGrid, 0, 1);       

            GameGrid.RowDefinitions.Add(new RowDefinition());
            GameGrid.RowDefinitions.Add(new RowDefinition());
            GameGrid.RowDefinitions.Add(new RowDefinition());

            GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            GameGrid.ColumnDefinitions.Add(new ColumnDefinition());

            ChatGrid.RowDefinitions.Add(new RowDefinition());
            ChatGrid.RowDefinitions.Add(new RowDefinition { MaxHeight = 90});
            ChatGrid.SetElementToGrid(_tbxMessage = new TextBox(), 1, 0);
            ChatGrid.SetElementToGrid(_btnSendMessage = new Button { Content = "send message",Command =  sendMessageComand , HorizontalAlignment = HorizontalAlignment.Right, Width = 70}, 1, 0);
            ChatGrid.SetElementToGrid(_privateChat=new ListView(), 0, 0);
            ChatGrid.SetElementToGrid(_btnExitGame = new Button { Content = "Exit",Command =  _exitGameCommand , HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top, Height=30, Width = 50}, 0, 0);
            #endregion

            for (int i = 0; i < board.Board.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < board.Board.Cells.GetLength(1); j++)
                {//HorizontalAlignment = HorizontalAlignment.Center , VerticalAlignment= VerticalAlignment.Center,
                    Rectangle R;
                    GameGrid.SetElementToGrid(R = new Rectangle { Stretch = Stretch.Fill, Fill = new SolidColorBrush(Colors.White), Stroke = new SolidColorBrush(Colors.Gold), StrokeThickness = 0 }, i, j); ;
                    _rectangles[i, j] = R;
                }
            }


        }

        private  bool CheckWinAfterEnemyPlay()
        {
            if (board.WinnerPlayer != PlayerCell.Empty)
            {
                OrderTheViewByNewBord();
                MessageBox.Show("Game Over You Are the LOSER");
                _gameClientManager.GameEnd(UserName);               
                return true;
            
            }
            return false;
        }

        private void RegisterToEvents()
        {
            //_chatClientManager.RegisterToSingleMsgRecievedEvent(OnPrivateMsgRecievedNotificated);
            _gameClientManager.RegisterToReceiveGame(OnGameReceive);
            _gameClientManager.RegisterToReceiveGameMessage(OnGameMessageReceive);
            _gameClientManager.RegisterToReceiveEndGameMessage(OnEndGameMessageReceive);
        }

        private void OnGameMessageReceive(string message)
        {
            if (board.X.UserName == UserName)
            {
                _privateChat.Items.Add($"{board.O.UserName} says {message}");
            }
            else
            {
                _privateChat.Items.Add($"{board.X.UserName} says {message}");
            }
        }

        private void OnEndGameMessageReceive(string message)
        {
            MessageBox.Show(message);
        }
        private void OnGameReceive(Game game)
        {
            board = game;            
            if (CheckWinAfterEnemyPlay()) // if true the enemy won 
            {
                
                MessageBox.Show("The game is over you can now close the game");
                return;
            }
            StartMouseDownEvent();
        }

        private void OrderTheViewByNewBord()
        {

            for (int i = 0; i < board.Board.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < board.Board.Cells.GetLength(1); j++)
                {                        
                     if(board.Board.Cells[i,j].CellColor!=PlayerCell.Empty)
                        GameGrid.SetElementToGrid(new Image { Source = new BitmapImage(new Uri(board.ReturnImageSourceToCellColor(board.Board.Cells[i, j].CellColor))) }, i, j);                    
                }
            }
        }
    }

    public interface IClosable
    {
        void Close();
    }



}


