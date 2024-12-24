using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Models
{
    public class ChessGame
    {
        private ChessPiece[,] board;
        public bool IsWhiteTurn { get; private set; }
        public Move LastMove { get; private set; }  // 마지막 이동 추적을 위해 추가
        protected ChessGame Game { get; set; }  // 게임 참조 추가




        public ChessGame()
        {
            board = new ChessPiece[8, 8];
            IsWhiteTurn = true;
            InitializeBoard();
        }
        public class Move
        {
            public Position From { get; set; }
            public Position To { get; set; }
            public ChessPiece Piece { get; set; }
            public bool IsTwoSquarePawnAdvance { get; set; }

            public Move(Position from, Position to, ChessPiece piece)
            {
                From = from;
                To = to;
                Piece = piece;
                IsTwoSquarePawnAdvance = piece is Pawn && Math.Abs(to.Row - from.Row) == 2;
            }
        }


        private void InitializeBoard()
        {
            // 폰 배치
            for (int col = 0; col < 8; col++)
            {
                board[1, col] = new Pawn(false, this);
                board[6, col] = new Pawn(true, this);
            }

            // 룩 배치
            board[0, 0] = new Rook(false, this);
            board[0, 7] = new Rook(false, this);
            board[7, 0] = new Rook(true, this);
            board[7, 7] = new Rook(true, this);

            // 나이트 배치
            board[0, 1] = new Knight(false, this);
            board[0, 6] = new Knight(false, this);
            board[7, 1] = new Knight(true, this);
            board[7, 6] = new Knight(true, this);

            // 비숍 배치
            board[0, 2] = new Bishop(false, this);
            board[0, 5] = new Bishop(false, this);
            board[7, 2] = new Bishop(true, this);
            board[7, 5] = new Bishop(true, this);

            // 퀸 배치
            board[0, 3] = new Queen(false, this);
            board[7, 3] = new Queen(true, this);

            // 킹 배치
            board[0, 4] = new King(false, this);
            board[7, 4] = new King(true, this);
        }

        public ChessPiece GetPiece(int row, int col)
        {
            return board[row, col];
        }

        public bool MovePiece(Position from, Position to)
        {
            var piece = board[from.Row, from.Col];
            if (piece == null || piece.IsWhite != IsWhiteTurn)
                return false;

            if (!piece.IsValidMove(from, to, board))
                return false;

            // 이동 기록 저장
            LastMove = new Move(from, to, piece);

            // 이동 실행
            board[to.Row, to.Col] = piece;
            board[from.Row, from.Col] = null;
            piece.HasMoved = true;

            // 턴 변경
            IsWhiteTurn = !IsWhiteTurn;

            return true;
        }

        public bool IsCheck()
        {
            // 현재 차례의 킹 위치 찾기
            Position? kingPosition = null;
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var piece = board[row, col];
                    if (piece is King && piece.IsWhite == IsWhiteTurn)
                    {
                        kingPosition = new Position(row, col);
                        break;
                    }
                }
                if (kingPosition.HasValue) break;
            }

            if (!kingPosition.HasValue)
                return false;

            // 모든 상대방 말에 대해 킹을 공격할 수 있는지 확인
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var piece = board[row, col];
                    if (piece != null && piece.IsWhite != IsWhiteTurn)
                    {
                        if (piece.IsValidMove(new Position(row, col), kingPosition.Value, board))
                            return true;
                    }
                }
            }

            return false;
        }

        public ChessPiece[,] GetBoard()
        {
            // 보드의 복사본을 반환하여 직접 수정을 방지
            ChessPiece[,] boardCopy = new ChessPiece[8, 8];
            Array.Copy(board, boardCopy, board.Length);
            return boardCopy;
        }
    }
}