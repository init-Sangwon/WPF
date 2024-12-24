using Chess.Helpers;
using Chess.Models;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;

namespace Chess.ViewModels
{
    public class ChessBoardViewModel : INotifyPropertyChanged
    {
        private ChessPiece[,] _board;
        private Position? _selectedPosition;
        private bool _isWhiteTurn = true;
        private ObservableCollection<Position> _validMoves;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsWhiteTurn
        {
            get => _isWhiteTurn;
            set
            {
                _isWhiteTurn = value;
                OnPropertyChanged(nameof(IsWhiteTurn));
                OnPropertyChanged(nameof(CurrentPlayerText));
            }
        }

        public string CurrentPlayerText => $"현재 차례: {(IsWhiteTurn ? "흰색" : "검은색")}";

        public ObservableCollection<ChessSquareViewModel> Squares { get; }

        public ICommand SquareClickCommand { get; }

        public ChessBoardViewModel()
        {
            _board = new ChessPiece[8, 8];
            Squares = new ObservableCollection<ChessSquareViewModel>();
            SquareClickCommand = new RelayCommand<Position>(OnSquareClick);
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int col = 0; col < 8; col++)
            {
                _board[1, col] = new Pawn(false); // 검은색 폰
                _board[6, col] = new Pawn(true);  // 흰색 폰
            }

            _board[0, 0] = _board[0, 7] = new Rook(false);
            _board[7, 0] = _board[7, 7] = new Rook(true);

            _board[0, 1] = _board[0, 6] = new Knight(false);
            _board[7, 1] = _board[7, 6] = new Knight(true);

            _board[0, 2] = _board[0, 5] = new Bishop(false);
            _board[7, 2] = _board[7, 5] = new Bishop(true);

            _board[0, 3] = new Queen(false);
            _board[0, 4] = new King(false);

            _board[7, 3] = new Queen(true);
            _board[7, 4] = new King(true);

            UpdateSquares();
        }

        private void OnSquareClick(Position position)
        {
            if (_selectedPosition == null)
            {
                var piece = _board[position.Row, position.Col];
                if (piece != null && piece.IsWhite == IsWhiteTurn)
                {
                    _selectedPosition = position;
                    _validMoves = new ObservableCollection<Position>(GetValidMoves(position));
                    UpdateSquares();
                }
            }
            else
            {
                var from = _selectedPosition.Value;
                var piece = _board[from.Row, from.Col];

                if (piece != null && _validMoves.Contains(position))
                {
                    MovePiece(from, position);
                    IsWhiteTurn = !IsWhiteTurn;

                    if (IsCheckmate())
                    {
                        MessageBox.Show($"{(IsWhiteTurn ? "검은색" : "흰색")} 승리!");
                    }
                    else if (IsCheck())
                    {
                        MessageBox.Show("체크!");
                    }
                }

                _selectedPosition = null;
                _validMoves.Clear();
                UpdateSquares();
            }
        }

        private ObservableCollection<Position> GetValidMoves(Position from)
        {
            var validMoves = new ObservableCollection<Position>();
            var piece = _board[from.Row, from.Col];

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var to = new Position(row, col);
                    if (piece.IsValidMove(from, to, _board) && !WouldResultInCheck(from, to))
                    {
                        validMoves.Add(to);
                    }
                }
            }
            return validMoves;
        }

        private bool WouldResultInCheck(Position from, Position to)
        {
            var clonedBoard = ChessHelper.CloneBoard(_board);

            var movingPiece = clonedBoard[from.Row, from.Col];
            clonedBoard[from.Row, from.Col] = null;
            clonedBoard[to.Row, to.Col] = movingPiece;

            return ChessHelper.IsSquareUnderAttack(FindKing(IsWhiteTurn), !IsWhiteTurn, clonedBoard);
        }

        private bool IsCheck()
        {
            var kingPosition = FindKing(IsWhiteTurn);
            return ChessHelper.IsSquareUnderAttack(kingPosition, !IsWhiteTurn, _board);
        }

        private Position FindKing(bool isWhite)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var piece = _board[row, col];
                    if (piece != null && piece.Type == PieceType.King && piece.IsWhite == isWhite)
                    {
                        return new Position(row, col);
                    }
                }
            }
            throw new InvalidOperationException("킹을 찾을 수 없습니다.");
        }

        private void MovePiece(Position from, Position to)
        {
            var movingPiece = _board[from.Row, from.Col];
            _board[to.Row, to.Col] = movingPiece;
            _board[from.Row, from.Col] = null;
            movingPiece.HasMoved = true;
            UpdateSquares();
        }

        private void UpdateSquares()
        {
            Squares.Clear();
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    var position = new Position(row, col);
                    var piece = _board[row, col];
                    var isSelected = _selectedPosition?.Equals(position) ?? false;
                    var isMoveTarget = _validMoves?.Contains(position) ?? false;

                    Squares.Add(new ChessSquareViewModel(
                        position,
                        piece?.Type ?? PieceType.None,
                        piece?.IsWhite ?? false,
                        isSelected,
                        isMoveTarget
                    ));
                }
            }
        }

        private bool IsCheckmate()
        {
            if (!IsCheck())
                return false;

            for (int fromRow = 0; fromRow < 8; fromRow++)
            {
                for (int fromCol = 0; fromCol < 8; fromCol++)
                {
                    var piece = _board[fromRow, fromCol];
                    if (piece != null && piece.IsWhite == IsWhiteTurn)
                    {
                        for (int toRow = 0; toRow < 8; toRow++)
                        {
                            for (int toCol = 0; toCol < 8; toCol++)
                            {
                                var from = new Position(fromRow, fromCol);
                                var to = new Position(toRow, toCol);

                                if (piece.IsValidMove(from, to, _board) && !WouldResultInCheck(from, to))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
