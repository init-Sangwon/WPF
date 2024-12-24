namespace Chess.Models
{
    public abstract class ChessPiece
    {
        public bool IsWhite { get; }
        public bool HasMoved { get; set; }
        public PieceType Type { get; }
        protected ChessGame Game { get; set; }  // 게임 참조 추가

        protected ChessPiece(PieceType type, bool isWhite, ChessGame game)
        {
            Type = type;
            IsWhite = isWhite;
            Game = game;
            HasMoved = false;
        }

        public abstract bool IsValidMove(Position from, Position to, ChessPiece[,] board);
        public abstract ChessPiece Clone();
    }

}
