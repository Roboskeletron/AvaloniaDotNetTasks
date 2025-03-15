namespace Chess.Domain;

public class Bishop : ChessPieceBase
{
    public Bishop(PieceColor color, (int X, int Y) position) : base(color, position)
    {
    }

    protected override bool CanMoveTo((int x, int y) position) => Math.Abs(Position.X - position.x) == Math.Abs(Position.Y - position.y);
}
