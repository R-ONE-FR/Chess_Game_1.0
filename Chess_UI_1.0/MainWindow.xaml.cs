using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Chess_Logic_1._0;

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
        }

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
                HandleMove(move);
            }
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
            Color color = Color.FromArgb(140, 45, 45, 45);

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
            HideHighlights();
            moveCache.Clear();
            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
            SetCursor(gameState.CurrentPlayer);
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