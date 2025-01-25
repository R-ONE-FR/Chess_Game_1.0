using Chess_Logic_1._0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_Logic_1._0
{
    public abstract class Piece {
        public abstract PieceType Type { get; }
        public abstract Player Color { get; }

        public bool HasMoved { get; set; } = false;

        public abstract Piece Copy();


    }
}
