using Avalonia.Logging;
using LogManager.Domain;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using LogsManager = LogManager.Domain.LogManager;

namespace LogManager.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly LogsManager _logManager;

    private int _logIndex;
    private readonly ObservableAsPropertyHelper<int> _logsCount;

    public int LogIndex { get => _logIndex; set => this.RaiseAndSetIfChanged(ref _logIndex, value); }

    public string Timestamp => _logManager[LogIndex].Timestamp.ToString();

    public string Level => _logManager[LogIndex].Level.ToString();

    public string Message => _logManager[LogIndex].Message;

    public int LogsCount => _logsCount.Value;

    public MainWindowViewModel()
    {
        _logManager = new LogsManager(new List<LogMessage>
        {
            new(DateTime.Parse("10/10/2024"), LogLevel.Information,  "This is an information log"),
            new(DateTime.Parse("11/11/2024"), LogLevel.Warning, "This is a warning log"),
            new(DateTime.Parse("1/12/2024"), LogLevel.Error, "This is an error log"),
            new(DateTime.Parse("8/9/2024"), LogLevel.Information, "Another info log"),
            new(DateTime.Parse("4/11/2024"), LogLevel.Warning, "Another warning log"),
            new(DateTime.Parse("6/11/2024"), LogLevel.Error, "Another error log"),
            new(DateTime.Parse("6/1/2024"), LogLevel.Information, "More information"),
            new(DateTime.Parse("1/15/2024"), LogLevel.Warning, "Yet another warning"),
            new(DateTime.Parse("3/16/2024"), LogLevel.Error, "Critical error encountered"),
            new(DateTime.Parse("2/11/2024"), LogLevel.Information, "Final info log")
        }.OrderBy(x => x.Timestamp));

        this.WhenAnyValue(x => x.LogIndex)
            .Subscribe(x =>
            {
                Logger.Sink!.Log(LogEventLevel.Information, nameof(MainWindowViewModel), this, "Index changed to {Index}", LogIndex);
                this.RaisePropertyChanged(nameof(Timestamp));
                this.RaisePropertyChanged(nameof(Level));
                this.RaisePropertyChanged(nameof(Message));
            });

        this.WhenAnyValue(x => x._logManager.LogsCount)
            .Select(x => x - 1)
            .ToProperty(this, x => x.LogsCount, out _logsCount);
    }
}
