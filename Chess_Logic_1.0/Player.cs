﻿namespace Chess_Logic_1._0
{
    public enum Player
    {
        None, White, Black

    }

    public static class PlayerExtensions
    {
        public static Player Opponnent(this Player player)
        {
            switch (player)
            {
                case Player.White:
                    return Player.Black;
                case Player.Black:
                    return Player.White;
                default:
                    return Player.None;
            }
        }
    }
}
