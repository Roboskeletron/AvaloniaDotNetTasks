namespace LogManager;

public record LogMessage(DateTime Timestamp, LogLevel Level, string Message);
