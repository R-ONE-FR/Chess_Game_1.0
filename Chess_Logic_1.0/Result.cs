namespace Chess_Logic_1._0
{
    public class Result
    {
        public Player Winner { get; }
        public EndReason Reason { get; }

        public Result(Player winner, EndReason reason)
        {
            Winner = winner;
            Reason = reason;
        }

        public static Result Win(Player winner) 
        {
            return new Result(winner, EndReason.CheckMate); 
        }

        public static Result Draw(EndReason reason) 
        {
            return new Result(Player.None, reason);
        }
    }
}
