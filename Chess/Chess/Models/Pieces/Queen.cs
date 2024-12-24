namespace Chess.Models
{
    public class Queen : ChessPiece
    {
        public Queen(bool isWhite, ChessGame game) : base(PieceType.Queen, isWhite, game) { }

        public override bool IsValidMove(Position from, Position to, ChessPiece[,] board)
        {
            var rook = new Rook(IsWhite, Game);
            var bishop = new Bishop(IsWhite, Game);

            return rook.IsValidMove(from, to, board) || bishop.IsValidMove(from, to, board);
        }

        public override ChessPiece Clone()
        {
            return new Queen(IsWhite, Game) { HasMoved = this.HasMoved };
        }
    }
}
