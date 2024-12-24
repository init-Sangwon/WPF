namespace Chess.Models
{
    public class Knight : ChessPiece
    {
        public Knight(bool isWhite, ChessGame game) : base(PieceType.Knight, isWhite, game) { }

        public override bool IsValidMove(Position from, Position to, ChessPiece[,] board)
        {
            int rowDiff = Math.Abs(from.Row - to.Row);
            int colDiff = Math.Abs(from.Col - to.Col);

            return rowDiff * colDiff == 2 && (board[to.Row, to.Col]?.IsWhite != IsWhite);
        }

        public override ChessPiece Clone()
        {
            return new Knight(IsWhite, Game) { HasMoved = this.HasMoved };
        }
    }
}
