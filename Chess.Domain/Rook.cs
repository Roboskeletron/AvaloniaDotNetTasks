namespace Chess.Domain;

public class Rook : ChessPieceBase
{
    public Rook(PieceColor color, (int X, int Y) position) : base(color, position)
    {
    }

    protected override bool CanMoveTo((int x, int y) position) =>
        Position.X == position.x ||
        Position.Y == position.y;
}
