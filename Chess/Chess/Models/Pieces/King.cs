using Chess.Helpers;
namespace Chess.Models
{
    public class King : ChessPiece
    {
        public King(bool isWhite, ChessGame game) : base(PieceType.King, isWhite, game) { }

        public override bool IsValidMove(Position from, Position to, ChessPiece[,] board)
        {
            // 일반적인 킹의 1칸 이동
            int rowDiff = Math.Abs(to.Row - from.Row);
            int colDiff = Math.Abs(to.Col - from.Col);

            // 일반 이동 (1칸)
            if (rowDiff <= 1 && colDiff <= 1)
            {
                var targetPiece = board[to.Row, to.Col];
                return targetPiece == null || targetPiece.IsWhite != this.IsWhite;
            }

            // 캐슬링 체크
            if (CanCastle(from, to, board))
            {
                // 실제 캐슬링 수행 (룩 이동)
                int rookFromCol = to.Col > from.Col ? 7 : 0; // 킹사이드/퀸사이드 룩 위치
                int rookToCol = to.Col > from.Col ? (from.Col + 1) : (from.Col - 1); // 룩의 새 위치

                // 룩 이동
                board[to.Row, rookToCol] = board[to.Row, rookFromCol];
                board[to.Row, rookFromCol] = null;

                if (board[to.Row, rookToCol] != null)
                {
                    board[to.Row, rookToCol].HasMoved = true;
                }

                return true;
            }

            return false;
        }

        public bool CanCastle(Position from, Position to, ChessPiece[,] board)
        {
            // 기본 조건 체크
            if (HasMoved || IsInCheck(from, from, board))  // 현재 위치에서 체크 상태인지 확인
            {
                return false;
            }

            // 킹의 시작 위치 확인
            int correctRow = IsWhite ? 7 : 0;
            if (from.Row != correctRow || from.Col != 4)
            {
                return false;
            }

            // 캐슬링 방향 확인
            bool isKingSide = to.Col == 6;
            bool isQueenSide = to.Col == 2;

            if (!isKingSide && !isQueenSide)
            {
                return false;
            }

            // 룩 위치와 상태 확인
            int rookCol = isKingSide ? 7 : 0;
            var rook = board[from.Row, rookCol] as Rook;

            if (rook == null || rook.IsWhite != IsWhite || rook.HasMoved)
            {
                return false;
            }

            // 경로상의 기물 확인
            int startCol = isKingSide ? 5 : 1;
            int endCol = isKingSide ? 6 : 3;

            for (int col = Math.Min(startCol, endCol); col <= Math.Max(startCol, endCol); col++)
            {
                if (board[from.Row, col] != null)
                {
                    return false;
                }
            }

            // 경로상의 체크 확인
            for (int col = from.Col; col != to.Col; col += (isKingSide ? 1 : -1))
            {
                var midPosition = new Position(from.Row, col);
                if (ChessHelper.IsSquareUnderAttack(midPosition, IsWhite, board))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsInCheck(Position from, Position to, ChessPiece[,] board)
        {
            return ChessHelper.IsSquareUnderAttack(to, IsWhite, board);
        }

        public override ChessPiece Clone()
        {
            return new King(IsWhite, Game) { HasMoved = this.HasMoved };
        }
    }
}