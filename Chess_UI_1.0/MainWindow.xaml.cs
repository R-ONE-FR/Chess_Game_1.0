using System.Windows;
using System.Windows.Controls;
using Chess_Logic_1._0;

namespace Chess_UI_1._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Image[,] pieceImages = new Image[8, 8];

        private GameState gameState;

        public MainWindow()
        {
            InitializeComponent();
            InitializedBoard();
            gameState = new GameState(Player.White, Board.Initial());
            DrawBoard(gameState.Board);
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