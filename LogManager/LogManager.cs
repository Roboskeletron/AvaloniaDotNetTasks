using System.Text;
using System.Text.Json;

namespace LogManager.Domain;

public class LogManager
{
    private readonly List<LogMessage> _logs;

    public LogMessage this[int index] => _logs[index];

    public LogManager(IEnumerable<LogMessage> logs)
    {
        _logs = logs.ToList();
    }

    public int LogsCount => _logs.Count;

    public IEnumerable<LogMessage> GetByLogLevel(LogLevel logLevel) =>
        _logs.Where(x => x.Level == logLevel).ToList();

    public IEnumerable<LogMessage> GetByDateRange(DateTime startDate, DateTime endDate) =>
        _logs.Where(x => x.Timestamp >= startDate && x.Timestamp <= endDate).ToList();

    public void Log(LogLevel level, string message) => _logs.Add(new LogMessage(DateTime.UtcNow, level, message));

    public void LogInfo(string message) => Log(LogLevel.Information, message);

    public void LogError(string message) => Log(LogLevel.Error, message);

    public void LogWarning(string message) => Log(LogLevel.Warning, message);

    public void SaveToFile(Stream fileStream)
    {
        var serializedData = JsonSerializer.Serialize(_logs);
        var rawData = Encoding.UTF8.GetBytes(serializedData);
        fileStream.Write(rawData, 0, rawData.Length);
    }

    public Task SaveToFileAsync(Stream fileStream, CancellationToken cancellationToken) =>
        JsonSerializer.SerializeAsync(fileStream, _logs, cancellationToken: cancellationToken);
}
