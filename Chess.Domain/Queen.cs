namespace Chess.Domain;

public class Queen : ChessPieceBase
{
    public Queen(PieceColor color, (int X, int Y) position) : base(color, position)
    {
    }

    protected override bool CanMoveTo((int x, int y) position) =>
        Position.X == position.x ||
        Position.Y == position.y ||
        Math.Abs(Position.X - position.x) == Math.Abs(Position.Y - position.y);
}
