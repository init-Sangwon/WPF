using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Chess.Models;

namespace Chess.ViewModels
{
    public class ChessSquareViewModel
    {
        public Position Position { get; }
        public PieceType PieceType { get; }
        public bool IsWhite { get; }
        public bool IsSelected { get; }
        public bool IsMoveTarget { get; }

        public Brush Background => IsSelected
            ? Brushes.Yellow
            : IsMoveTarget
                ? Brushes.LightBlue
                : ((Position.Row + Position.Col) % 2 == 0)
                    ? Brushes.White
                    : Brushes.Gray;

        public ImageSource PieceImage { get; }

        public ChessSquareViewModel(Position position, PieceType pieceType, bool isWhite, bool isSelected, bool isMoveTarget = false)
        {
            Position = position;
            PieceType = pieceType;
            IsWhite = isWhite;
            IsSelected = isSelected;
            IsMoveTarget = isMoveTarget;

            if (pieceType != PieceType.None)
            {
                string color = isWhite ? "white" : "black";
                string pieceName = pieceType.ToString().ToLower();
                string imagePath = $"pack://application:,,,/Images/{color}_{pieceName}.png";

                try
                {
                    PieceImage = new BitmapImage(new Uri(imagePath));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"이미지 로드 실패: {imagePath}, 오류: {ex.Message}");
                    PieceImage = null;
                }
            }
        }
    }
}
