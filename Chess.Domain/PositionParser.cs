namespace Chess.Domain;

public static class PositionParser
{
    public static bool TryParse(string? notation, out (int X, int Y) position)
    {
        try
        {
            position = Parse(notation);
            return true;
        }
        catch (FormatException)
        {
            position = default;
        }

        return false;
    }

    public static (int X, int Y) Parse(string? notation)
    {
        if (string.IsNullOrEmpty(notation) || notation.Length != 2)
            throw new FormatException("Invalid notation. Notation must be 2 characters long (e.g., 'A1').");

        notation = notation.ToUpper();

        char fileChar = notation[0];
        char rankChar = notation[1];

        int x = fileChar - 'A';
        if (x < 0 || x > 7)
            throw new FormatException("Invalid file. File must be between 'A' and 'H'.");

        int y = rankChar - '1';
        if (y < 0 || y > 7)
            throw new FormatException("Invalid rank. Rank must be between '1' and '8'.");

        return (x, y);
    }

    public static string ToNotation(this (int X, int Y) position)
    {
        if (position.X < 0 || position.X > 7 || position.Y < 0 || position.Y > 7)
            throw new ArgumentException("Invalid position. Coordinates must be between (0, 0) and (7, 7).");

        char fileChar = (char)('A' + position.X);
        char rankChar = (char)('1' + position.Y);

        return $"{fileChar}{rankChar}";
    }
}