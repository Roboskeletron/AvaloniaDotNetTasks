namespace LogManager.Domain;

public record LogMessage(DateTime Timestamp, LogLevel Level, string Message);
