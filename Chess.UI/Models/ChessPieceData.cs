using Chess.Domain;

namespace Chess.UI.Models;

public record ChessPieceData
{
    public string? InitialPosition { get; set; }

    public string? MoveToPosition { get; set; }

    public PieceColor Color { get; set; }

    public ChessPieceBase? ChessPiece { get; set; }
}
