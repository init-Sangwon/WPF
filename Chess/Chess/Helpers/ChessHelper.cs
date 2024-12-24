using Chess.Models;

namespace Chess.Helpers
{
    public static class ChessHelper
    {
        public static bool IsSquareUnderAttack(Position position, bool isAttackerWhite, ChessPiece[,] board)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var piece = board[row, col];
                    if (piece != null && piece.IsWhite == isAttackerWhite)
                    {
                        if (piece.IsValidMove(new Position(row, col), position, board))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static ChessPiece[,] CloneBoard(ChessPiece[,] board)
        {
            var clone = new ChessPiece[8, 8];
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    clone[row, col] = board[row, col]?.Clone();
                }
            }
            return clone;
        }
    }
}
