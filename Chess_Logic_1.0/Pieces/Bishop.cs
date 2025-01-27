﻿using Chess_Logic_1._0.Moves;

namespace Chess_Logic_1._0
{
    public class Bishop : Piece
    {
        public override PieceType Type => PieceType.Bishop;

        public override Player Color { get; }

        private static readonly Direction[] dirs = new Direction[]
        {
            Direction.NortWest,
            Direction.SouthWest,
            Direction.NortEast,
            Direction.SouthEast
        };

        public Bishop(Player color)
        {
            Color = color;
        }

        public override Piece Copy()
        {
            Bishop copy = new Bishop(Color);
            copy.HasMoved = HasMoved;
            return copy;
        }

        public override IEnumerable<Move> GetMoves(Position from, Board board)
        {
            return MovePositionsInDirs(from, board, dirs).Select(to => new NormalMove(from, to));
        }
    }
}
