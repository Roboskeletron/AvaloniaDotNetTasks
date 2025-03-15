namespace Chess.Domain;

public abstract class ChessPieceBase
{
    public (int X, int Y) Position { get; private set; }

    public PieceColor Color { get; private set; }

    protected ChessPieceBase(PieceColor color, (int X, int Y) position)
    {
        Color = color;
        Position = position;
    }

    protected ChessPieceBase(PieceColor color, string positionNotation) : this(color, PositionParser.Parse(positionNotation))
    {
    }

    public void Move((int x, int y) position)
    {
        if (!CanMoveTo(position)) return;

        Position = position;
    }

    protected abstract bool CanMoveTo((int x, int y) position);
}
