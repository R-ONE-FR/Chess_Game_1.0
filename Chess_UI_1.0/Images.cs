using System.Windows.Media;
using System.Windows.Media.Imaging;
using Chess_Logic_1._0;

namespace Chess_UI_1._0
{
    public static class Images
    {
        private static readonly Dictionary<PieceType, ImageSource> whiteSources = new()
        {
            { PieceType.Pawn, LoadImage("Assets/wp.png") },
            { PieceType.Bishop, LoadImage("Assets/wb.png") },
            { PieceType.Knight, LoadImage("Assets/wk.png") },
            { PieceType.Rook, LoadImage("Assets/wr.png") },
            { PieceType.Queen, LoadImage("Assets/wq.png") },
            { PieceType.King, LoadImage("Assets/wk.png") }
        };

        private static readonly Dictionary<PieceType, ImageSource> blackSources = new()
        {
            { PieceType.Pawn, LoadImage("Assets/bp.png") },
            { PieceType.Bishop, LoadImage("Assets/bb.png") },
            { PieceType.Knight, LoadImage("Assets/bk.png") },
            { PieceType.Rook, LoadImage("Assets/br.png") },
            { PieceType.Queen, LoadImage("Assets/bq.png") },
            { PieceType.King, LoadImage("Assets/bk.png") }
        };

        private static ImageSource LoadImage(string filePath)
        {
            return new BitmapImage(new Uri(filePath, UriKind.Relative));
        }

        public static ImageSource GetImage(Player color, PieceType type)
        {
            return color
                switch
            {
                Player.White => whiteSources[type],
                Player.Black => blackSources[type],
                _ => null
            };

        }

        public static ImageSource GetImage(Piece piece)
        {
            if (piece == null) return null;
            return GetImage(piece.Color, piece.Type);
        }
    }
}
