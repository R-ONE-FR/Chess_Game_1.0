﻿using Chess_Logic_1._0.Moves;

namespace Chess_Logic_1._0
{
    public class Board
    {
        private readonly Piece[,] pieces = new Piece[8, 8];

        private readonly Dictionary<Player, Position> pawnSkipPosition = new Dictionary<Player, Position>
        {
            { Player.White, null },
            { Player.Black, null }
        };


        public Piece this[int row, int col]
        {
            get { return pieces[row, col]; }
            set { pieces[row, col] = value; }
        }

        public Piece this[Position pos]
        {
            get { return this[pos.Row, pos.Column]; }
            set { this[pos.Row, pos.Column] = value; }
        }

        public Position GetPawnSkipPosition(Player player)
        {
            return pawnSkipPosition[player];
        }

        public void SetPawnSkipPosition(Player player, Position pos)
        {
            pawnSkipPosition[player] = pos;
        }

        public static Board Initial()
        {
            Board board = new Board();
            board.AddStartPiece();
            return board;
        }

        private void AddStartPiece()
        {
            this[0, 0] = new Rook(Player.Black);
            this[0, 1] = new Knight(Player.Black);
            this[0, 2] = new Bishop(Player.Black);
            this[0, 3] = new Queen(Player.Black);
            this[0, 4] = new King(Player.Black);
            this[0, 5] = new Bishop(Player.Black);
            this[0, 6] = new Knight(Player.Black);
            this[0, 7] = new Rook(Player.Black);

            this[7, 0] = new Rook(Player.White);
            this[7, 1] = new Knight(Player.White);
            this[7, 2] = new Bishop(Player.White);
            this[7, 3] = new Queen(Player.White);
            this[7, 4] = new King(Player.White);
            this[7, 5] = new Bishop(Player.White);
            this[7, 6] = new Knight(Player.White);
            this[7, 7] = new Rook(Player.White);

            for (int i = 0; i < 8; i++)
            {
                this[1, i] = new Pawn(Player.Black);
                this[6, i] = new Pawn(Player.White);
            }
            /*
            this[1, 0] = new Pawn(Player.Black);
            this[1, 1] = new Pawn(Player.Black);
            this[1, 2] = new Pawn(Player.Black);
            this[1, 3] = new Pawn(Player.Black);
            this[1, 4] = new Pawn(Player.Black);
            this[1, 5] = new Pawn(Player.Black);
            this[1, 6] = new Pawn(Player.Black);
            this[1, 7] = new Pawn(Player.Black);


            this[6, 0] = new Pawn(Player.White);
            this[6, 1] = new Pawn(Player.White);
            this[6, 2] = new Pawn(Player.White);
            this[6, 3] = new Pawn(Player.White);
            this[6, 4] = new Pawn(Player.Black);
            this[6, 5] = new Pawn(Player.White);
            this[6, 6] = new Pawn(Player.White);
            this[6, 7] = new Pawn(Player.White);
            */
        }

        public static bool IsInside(Position pos)
        {
            return pos.Row >= 0 && pos.Row < 8 && pos.Column >= 0 && pos.Column < 8;
        }

        public bool IsEmpty(Position pos)
        {
            return this[pos] == null;
        }

        public IEnumerable<Position> PiecePositions()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Position pos = new Position(i, j);
                    if (!IsEmpty(pos))
                    {
                        yield return pos;
                    }
                }
            }
        }

        public IEnumerable<Position> PiecePositionsFor(Player player)
        {
            return PiecePositions().Where(pos => this[pos].Color == player);
        }

        public bool IsInCheck(Player player)
        {
            return PiecePositionsFor(player.Opponnent()).Any(pos =>
            {
                Piece piece = this[pos];
                return piece.CanCaptureOpponentKing(pos, this);
            });
        }

        public Board Copy()
        {
            Board copy = new Board();
            foreach (Position pos in PiecePositions())
            {
                copy[pos] = this[pos].Copy();
            }

            return copy;
        }

        public Counting CountPiece()
        {
            Counting counting = new Counting();

            foreach (Position pos in PiecePositions())
            {
                Piece piece =this[pos];
                counting.Increment(piece.Color, piece.Type);
            }

            return counting;
        }

        public bool InsufficientMaterial()
        {
            Counting counting = CountPiece();

            return IsKingVKing(counting) || IsKingBishopVKing(counting) || IsKingKnightVKin(counting) ||
                IsKingBishopVKingBishop(counting);
        }

        private static bool IsKingVKing(Counting counting)
        {
            return counting.TotalCount == 2; // Deux pieces restantes == deux rois 
        }

        private static bool IsKingBishopVKing(Counting counting)
        {
            return counting.TotalCount == 3 && (counting.White(PieceType.Bishop) == 1 || counting.Black(PieceType.Bishop) == 1);
        }
        private static bool IsKingKnightVKin(Counting counting)
        {
            return counting.TotalCount == 3 && (counting.White(PieceType.Knight) == 1 || counting.Black(PieceType.Knight) == 1);
        }

        private bool IsKingBishopVKingBishop(Counting counting)
        {
            if (counting.TotalCount != 4)
            {
                return false;
            }
            if (counting.White(PieceType.Bishop) != 1 || counting.Black(PieceType.Bishop) != 1)
            {
                return false;
            }

            Position wBishop = FindPiece(Player.White, PieceType.Bishop);
            Position bBishop = FindPiece(Player.Black, PieceType.Bishop);

            return wBishop.SquareColor() == bBishop.SquareColor();
        }

        private Position FindPiece(Player color, PieceType type)
        {
            return PiecePositionsFor(color).First(pos => this[pos].Type == type);
        }

        private bool IsUnmovedKingAndRook(Position kingPos, Position rookPos)
        {
            if(IsEmpty(kingPos) || IsEmpty(rookPos))
            {
                return false;
            }

            Piece king = this[kingPos];
            Piece rook = this[rookPos];

            return king.Type == PieceType.King && rook.Type == PieceType.Rook && !king.HasMoved && !rook.HasMoved;
        }

        public bool CastleRightKS(Player player)
        {
            return player switch
            {
                Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 7)),
                Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 7)),
                _ => false
            };
        }

        public bool CastleRightQS(Player player)
        {
            return player switch
            {
                Player.White => IsUnmovedKingAndRook(new Position(7, 4), new Position(7, 0)),
                Player.Black => IsUnmovedKingAndRook(new Position(0, 4), new Position(0, 0)),
                _ => false
            };
        }

        private bool HasPawnInPosition(Player player, Position[] pawnPositions, Position skipPos)
        {
            foreach (Position pos in pawnPositions.Where(IsInside))
            {
                Piece piece = this[pos];
                if (piece == null || piece.Color != player || piece.Type != PieceType.Pawn)
                {
                    continue;
                }

                EnPassant move = new EnPassant(pos, skipPos);
                if (move.IsLegal(this))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanCaptureEnPassant(Player player)
        {
            Position skipPos = GetPawnSkipPosition(player.Opponnent());

            if (skipPos == null)
            {
                return false;
            }

            Position[] pawnPositions = player switch
            {
                Player.White => new Position[] { skipPos + Direction.SouthWest, skipPos + Direction.SouthEast },
                Player.Black => new Position[] { skipPos + Direction.NorthWest, skipPos + Direction.NorthEast },
                _ => Array.Empty<Position>()
            };

            return HasPawnInPosition(player, pawnPositions, skipPos);
        }
    }
}