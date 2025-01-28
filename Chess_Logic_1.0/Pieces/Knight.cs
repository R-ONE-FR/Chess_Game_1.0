namespace Chess_Logic_1._0
{
    public class Knight : Piece
    {
        public override PieceType Type => PieceType.Knight;

        public override Player Color { get; }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.NortWest,
            Direction.SouthWest,
            Direction.NortEast,
            Direction.SouthEast
        };

        public Knight(Player color)
        {
            Color = color;
        }

        public override Piece Copy()
        {
            Knight copy = new Knight(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

    }
}
