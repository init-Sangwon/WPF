namespace Chess.Models
{
    public class Pawn : ChessPiece
    {
        public Pawn(bool isWhite, ChessGame game) : base(PieceType.Pawn, isWhite, game) { }

        public override bool IsValidMove(Position from, Position to, ChessPiece[,] board)
        {
            int direction = IsWhite ? -1 : 1;
            int startRow = IsWhite ? 6 : 1;

            // 직선 이동 (1칸 전진)
            if (to.Col == from.Col && to.Row == from.Row + direction && board[to.Row, to.Col] == null)
            {
                return true;
            }

            // 첫 번째 이동 시 2칸 전진
            if (to.Col == from.Col && to.Row == from.Row + 2 * direction &&
                from.Row == startRow && board[to.Row, to.Col] == null &&
                board[from.Row + direction, from.Col] == null)
            {
                return true;
            }

            // 대각선 이동
            if (Math.Abs(to.Col - from.Col) == 1 && to.Row == from.Row + direction)
            {
                var targetPiece = board[to.Row, to.Col];

                // 일반 대각선 공격
                if (targetPiece != null && targetPiece.IsWhite != this.IsWhite)
                {
                    return true;
                }

                // 앙파상
                var adjacentPiece = board[from.Row, to.Col];
                if (targetPiece == null &&
                    adjacentPiece is Pawn &&
                    adjacentPiece.IsWhite != this.IsWhite &&
                    (from.Row == (IsWhite ? 3 : 4)))
                {
                    var lastMove = Game.LastMove;
                    if (lastMove != null &&
                        lastMove.Piece == adjacentPiece &&
                        lastMove.IsTwoSquarePawnAdvance)
                    {
                        board[from.Row, to.Col] = null;  // 앙파상으로 잡힌 폰 제거
                        return true;
                    }
                }
            }

            return false;
        }

        public override ChessPiece Clone()
        {
            return new Pawn(IsWhite, Game) { HasMoved = this.HasMoved };
        }
    }
}