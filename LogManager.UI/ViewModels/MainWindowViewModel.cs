﻿using Avalonia.Platform.Storage;
using LogManager.Domain;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using LogsManager = LogManager.Domain.LogManager;

namespace LogManager.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly LogsManager _logManager;

    private int _logIndex;
    private LogLevel _logLevelFilter;
    private string? _startDateString;
    private string? _endDateString;
    private IEnumerable<LogMessage> _filteredByTimeRange = [];

    private readonly ObservableAsPropertyHelper<int> _logsCount;
    private readonly ObservableAsPropertyHelper<string> _timestamp;
    private readonly ObservableAsPropertyHelper<string> _level;
    private readonly ObservableAsPropertyHelper<string> _message;
    private readonly ObservableAsPropertyHelper<IEnumerable<LogMessage>> _filteredByLevel;

    public int LogIndex { get => _logIndex; set => this.RaiseAndSetIfChanged(ref _logIndex, value); }

    public string Timestamp => _timestamp.Value;

    public string Level => _level.Value;

    public string Message => _message.Value;

    public int LogsCount => _logsCount.Value;

    public LogLevel LogLevelFilter { get => _logLevelFilter; set => this.RaiseAndSetIfChanged(ref _logLevelFilter, value); }

    public IEnumerable<LogMessage> FilteredByLevel => _filteredByLevel.Value;

    public IEnumerable<LogMessage> FilteredByTimeRange { get => _filteredByTimeRange; set => this.RaiseAndSetIfChanged(ref _filteredByTimeRange, value); }

    public ReactiveCommand<Unit, Unit> SearchByTimeRangeCommand { get; }

    public string? StartDateString { get => _startDateString; set => this.RaiseAndSetIfChanged(ref _startDateString, value); }

    public string? EndDateString { get => _endDateString; set => this.RaiseAndSetIfChanged(ref _endDateString, value); }

    public ReactiveCommand<Unit, Unit> SaveLogsCommand { get; }

    public MainWindowViewModel(IStorageProvider storageProvider)
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
            new(DateTime.Parse("15/1/2024"), LogLevel.Warning, "Yet another warning"),
            new(DateTime.Parse("16/3/2024"), LogLevel.Error, "Critical error encountered"),
            new(DateTime.Parse("11/2/2024"), LogLevel.Information, "Final info log")
        }.OrderBy(x => x.Timestamp));

        this.WhenAnyValue(x => x.LogIndex)
            .Select(x => _logManager[x].Timestamp.ToString())
            .ToProperty(this, x => x.Timestamp, out _timestamp);

        this.WhenAnyValue(x => x.LogIndex)
            .Select(x => _logManager[x].Level.ToString())
            .ToProperty(this, x => x.Level, out _level);

        this.WhenAnyValue(x => x.LogIndex)
            .Select(x => _logManager[x].Message)
            .ToProperty(this, x => x.Message, out _message);

        this.WhenAnyValue(x => x._logManager.LogsCount)
            .Select(x => x - 1)
            .ToProperty(this, x => x.LogsCount, out _logsCount);

        this.WhenAnyValue(x => x.LogLevelFilter)
            .Select(x => _logManager.GetByLogLevel(x))
            .ToProperty(this, x => x.FilteredByLevel, out _filteredByLevel);

        SearchByTimeRangeCommand = ReactiveCommand.Create(() =>
        {
            if (!(DateTime.TryParse(StartDateString, out var startDate) && DateTime.TryParse(EndDateString, out var endDate)))
            {
                return;
            }

            FilteredByTimeRange = _logManager.GetByDateRange(startDate, endDate);
        });

        SaveLogsCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                FileTypeChoices = [new FilePickerFileType("json") {
                    MimeTypes = ["application/json"],
                    Patterns = ["*.json"]
                }]
            });

            if (file == null)
            {
                return;
            }

            using var stream = await file.OpenWriteAsync();

            await _logManager.SaveToFileAsync(stream, default);
        });

        FilteredByTimeRange = _logManager.GetByDateRange(DateTime.MinValue, DateTime.MaxValue);
    }
}
