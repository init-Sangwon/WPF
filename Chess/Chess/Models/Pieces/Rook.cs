namespace Chess.Models
{
    public class Rook : ChessPiece
    {
        public Rook(bool isWhite, ChessGame game) : base(PieceType.Rook, isWhite, game) { }

        public override bool IsValidMove(Position from, Position to, ChessPiece[,] board)
        {
            if (from.Row == to.Row || from.Col == to.Col)
            {
                int rowStep = to.Row > from.Row ? 1 : (to.Row < from.Row ? -1 : 0);
                int colStep = to.Col > from.Col ? 1 : (to.Col < from.Col ? -1 : 0);

                int currentRow = from.Row + rowStep;
                int currentCol = from.Col + colStep;

                while (currentRow != to.Row || currentCol != to.Col)
                {
                    if (board[currentRow, currentCol] != null)
                    {
                        return false;
                    }
                    currentRow += rowStep;
                    currentCol += colStep;
                }

                return board[to.Row, to.Col]?.IsWhite != IsWhite;
            }
            return false;
        }

        // Clone 메서드 구현
        public override ChessPiece Clone()
        {
            return new Rook(IsWhite, Game) { HasMoved = this.HasMoved };
        }
    }
}
