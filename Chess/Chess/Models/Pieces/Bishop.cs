using System;

namespace Chess.Models
{
    public class Bishop : ChessPiece
    {
        public Bishop(bool isWhite, ChessGame game) : base(PieceType.Bishop, isWhite, game) { }

        public override bool IsValidMove(Position from, Position to, ChessPiece[,] board)
        {
            int rowDiff = Math.Abs(to.Row - from.Row);
            int colDiff = Math.Abs(to.Col - from.Col);

            if (rowDiff != colDiff)
                return false;

            int rowStep = to.Row > from.Row ? 1 : -1;
            int colStep = to.Col > from.Col ? 1 : -1;

            for (int i = 1; i < rowDiff; i++)
            {
                if (board[from.Row + i * rowStep, from.Col + i * colStep] != null)
                    return false;
            }

            var targetPiece = board[to.Row, to.Col];
            return targetPiece == null || targetPiece.IsWhite != this.IsWhite;
        }

        // Clone 메서드를 올바르게 override하여 체스말 복제
        public override ChessPiece Clone()
        {
            return new Bishop(IsWhite, Game) { HasMoved = this.HasMoved };
        }
    }
}
