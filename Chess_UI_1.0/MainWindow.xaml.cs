﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;
using Chess_Logic_1._0;
using System.IO;

namespace Chess_UI_1._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Image[,] pieceImages = new Image[8, 8];
        private readonly Ellipse[,] highlights = new Ellipse[8, 8];
        private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();

        private GameState gameState;
        private Position selectedPos = null;

        public MainWindow()
        {
            InitializeComponent();
            InitializedBoard();
            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
            //Stockfish test = new Stockfish();
            StartStockfish();

            //Process ebc = test.StartStockfish();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e) => Close();
        private void BtnMaximize_Click(object sender, RoutedEventArgs e) => this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        private void BtnMinimize_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;
    

        private void InitializedBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Image image = new Image();
                    pieceImages[i, j] = image;
                    PieceGrid.Children.Add(image);

                    Ellipse highlight = new Ellipse();
                    highlight.Width = 45;  // Largeur de l'ellipse
                    highlight.Height = 45; // Hauteur de l'ellipse
                    highlight.Fill = new SolidColorBrush(Colors.Transparent); // Optionnel

                    highlights[i, j] = highlight;
                    HighlightGrid.Children.Add(highlight);
                }
            }
        }


        private void DrawBoard(Board board)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece piece = board[i, j];
                    pieceImages[i, j].Source = Images.GetImage(piece);
                }
            }
        }

        private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsMenuOnScreen())
            {
                return;
            }

            Point point = e.GetPosition(BoardGrid);
            Position pos = ToSquarePosition(point);

            if(selectedPos == null)
            {
                OnFromPositionSelected(pos);
            }
            else
            {
                OnToPostitionSelected(pos);
            }
        }

        private Position ToSquarePosition(Point point)
        {
            double squareSize = BoardGrid.ActualWidth / 8;
            int col = (int)(point.X / squareSize);
            int row = (int)(point.Y / squareSize);
            return new Position(row, col);
        }

        private void OnFromPositionSelected(Position pos)
        {
            IEnumerable<Move> moves = gameState.LegalMovesForPiece(pos);

            if(moves.Any())
            {
                selectedPos = pos;
                CacheMoves(moves);
                ShowHighlights();
            }
        }   

        private void OnToPostitionSelected(Position pos)
        {
            selectedPos = null;
            HideHighlights();

            if(moveCache.TryGetValue(pos, out Move move))
            {
                if (move.Type == MoveType.PawnPromotion)
                {
                    HandlePromotion(move.FromPos, move.ToPos);
                }
                else
                {
                    HandleMove(move);
                }
            }
        }

        private void HandlePromotion(Position from, Position to)
        {
            pieceImages[to.Row, to.Column].Source = Images.GetImage(gameState.CurrentPlayer, PieceType.Pawn);
            pieceImages[from.Row, from.Column].Source = null;

            PromotionMenu promMenu = new PromotionMenu(gameState.CurrentPlayer);
            MenuContainer.Content = promMenu;

            promMenu.PieceSelected += type =>
            {
                MenuContainer.Content = null;
                Move promMove = new PawnPromotion(from, to, type);
                HandleMove(promMove);
            };
        }

        private void HandleMove(Move move)
        {
            gameState.MakeMove(move);
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);

            if (gameState.IsGameOver())
            {
                ShowGameOver();
            }
        }

        private void CacheMoves(IEnumerable<Move> moves)
        {
            moveCache.Clear();

            foreach (Move move in moves)
            {
                moveCache[move.ToPos] = move;
            }
        }

        private void ShowHighlights()
        {
            Color color = Color.FromArgb(200, 22, 22, 22);

            foreach (Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = new SolidColorBrush(color);
            }
        }

        private void HideHighlights()
        {
            foreach (Position to in moveCache.Keys)
            {
                highlights[to.Row, to.Column].Fill = Brushes.Transparent;
            }
        }

        private void SetCursor(Player player)
        {
            if (player == Player.White) 
            {
                Cursor = ChessCursors.WhiteCursor;
            }
            else
            {
                Cursor = ChessCursors.BlackCursor;
            }
        }

        private bool IsMenuOnScreen()
        {
            return MenuContainer.Content != null;
        }

        private void ShowGameOver()
        {
            GameOverMenu gameOverMenu = new GameOverMenu(gameState);
            MenuContainer.Content = gameOverMenu;

            gameOverMenu.OptionSelected += option =>
            {
                if (option == Option.Restart)
                {
                    MenuContainer.Content = null;
                    RestartGame();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            };
        }

        private void RestartGame()
        {
            selectedPos = null;
            HideHighlights();
            moveCache.Clear();
            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
        }

        private void Winow_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsMenuOnScreen() && e.Key == Key.Escape)
            {
                ShowPauseMenu();
            }
        } 

        private void ShowPauseMenu()
        {
            PauseMenu pauseMenu = new PauseMenu();
            MenuContainer.Content = pauseMenu;

            pauseMenu.OptionSelected += option =>
            {
                MenuContainer.Content = null;

                if (option == Option.Restart)
                {
                    RestartGame();
                }
            };
        }




        private Process stockfishProcess;
        private StreamWriter stockfishInput;
        private StreamReader stockfishOutput;

        private void StartStockfish()
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "stockfish-windows-x86-64-avx2.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            stockfishProcess = new Process { StartInfo = psi };
            stockfishProcess.Start();

            stockfishInput = stockfishProcess.StandardInput;
            stockfishOutput = stockfishProcess.StandardOutput;

            // Lire les réponses de Stockfish en arrière-plan
            Task.Run(() => ReadStockfishOutput());
        }

        private async Task ReadStockfishOutput()
        {
            string response;
            while ((response = await stockfishOutput.ReadLineAsync()) != null)
            {
                // Mettre à jour l'UI en utilisant Dispatcher.Invoke
                Dispatcher.Invoke(() => outputTextBox.AppendText(response + Environment.NewLine));
                StateString test2 = new StateString(gameState.CurrentPlayer, gameState.Board);
                Dispatcher.Invoke(() => outputTextBox.AppendText(test2.ToString()));
            }
        }

        private async void SendCommand(object sender, RoutedEventArgs e)
        {
            string command = commandTextBox.Text;
            if (!string.IsNullOrEmpty(command) && stockfishProcess != null && !stockfishProcess.HasExited)
            {
                await stockfishInput.WriteLineAsync(command);
                commandTextBox.Clear();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (stockfishProcess != null && !stockfishProcess.HasExited)
            {
                stockfishProcess.Kill();
                stockfishProcess.Dispose();
            }
        }
    }
}

/*
 <Border CornerRadius="20" BorderThickness="1" Background="{StaticResource SecondaryBackground}" BorderBrush="{StaticResource BorderGradient}">

<!--Barre de Controle-->
<Border CornerRadius="0,10,0,0" Grid.Row="0">

    <StackPanel x:Name="ControlBar" Grid.Row="0" Orientation="Horizontal" FlowDirection="RightToLeft" Background="Transparent" Margin="0,0,5,0" MouseLeftButtonDown="ControlBar_MouseLeftButtonDown">

        <Button x:Name="BtnClose" Style="{StaticResource ctrlBtn}" Tag="{StaticResource close}" Click="BtnClose_Click">
            <fa:IconImage Icon="Xmark" Style="{StaticResource ctrlBtnIcon}"/>
        </Button>

        <Button x:Name="BtnMaximize" Style="{StaticResource ctrlBtn}" Tag="{StaticResource hover}" Click="BtnMaximize_Click">
            <fa:IconImage Icon="WindowMaximize" Style="{StaticResource ctrlBtnIcon}"/>
        </Button>

        <Button x:Name="BtnMinimize" Style="{StaticResource ctrlBtn}" Tag="{StaticResource hover}" Click="BtnMinimize_Click">
            <fa:IconImage Icon="WindowMinimize" Width="12" Style="{StaticResource ctrlBtnIcon}"/>
        </Button>
    </StackPanel>

</Border>
<Grid Background="{StaticResource PrimaryBackground}">
    <Image Margin="25,25,935,25" Source="/Assets/Board.png" Stretch="Fill"/>

</Grid>
</Border>

 
 
 */