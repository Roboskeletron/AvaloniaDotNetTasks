using System.Text;
using System.Text.Json;

namespace LogManager;

public class LogManager
{
    private readonly List<LogMessage> _logs = [];

    public IReadOnlyList<LogMessage> Logs => _logs;

    public IReadOnlyList<LogMessage> GetByLogLevel(LogLevel logLevel) =>
        Logs.Where(x => x.Level == logLevel).ToList();

    public IReadOnlyList<LogMessage> GetByTimeInterval(DateTime startDate, DateTime endDate) =>
        Logs.Where(x => x.Timestamp >= startDate && x.Timestamp <= endDate).ToList();

    public void SaveToFile(FileStream fileStream)
    {
        var serializedData = JsonSerializer.Serialize(Logs);
        var rawData = Encoding.UTF8.GetBytes(serializedData);
        fileStream.Write(rawData, 0, rawData.Length);
    }

    public Task SaveToFileAsync(FileStream fileStream, CancellationToken cancellationToken) =>
        JsonSerializer.SerializeAsync(fileStream, Logs, cancellationToken: cancellationToken);
}
