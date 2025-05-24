using Chess.Domain;
using Chess.Reflection.UI.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reflection;

namespace Chess.Reflection.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _libraryPath = "";
    private string? _selectedClassName;
    private Type? _selectedClassType;
    private DynamicMethodInfo? _selectedMethod;
    private object? _instance;
    private string _actionMessage = "";
    private ObservableCollection<string> _classes;
    private ObservableCollection<DynamicMethodInfo> _methods;

    private PieceColor _selectedColor;
    private string _initialPositionString = "A1"; // Default initial position

    public string LibraryPath
    {
        get => _libraryPath;
        set => this.RaiseAndSetIfChanged(ref _libraryPath, value);
    }

    public ObservableCollection<string> Classes
    {
        get => _classes;
        set => this.RaiseAndSetIfChanged(ref _classes, value);
    }

    public string? SelectedClassName
    {
        get => _selectedClassName;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedClassName, value);
            UpdateSelectedClassType();
            // Instance creation is now manual via CreateInstanceCommand
            _instance = null;
            Methods.Clear();
            SelectedMethod = null;
            if (_selectedClassType != null)
            {
                ActionMessage = "Select color, initial position, and click 'Create Piece'.";
            }
        }
    }

    public ObservableCollection<PieceColor> AvailableColors { get; }

    public PieceColor SelectedColor
    {
        get => _selectedColor;
        set => this.RaiseAndSetIfChanged(ref _selectedColor, value);
    }

    public string InitialPositionString
    {
        get => _initialPositionString;
        set => this.RaiseAndSetIfChanged(ref _initialPositionString, value);
    }

    public ObservableCollection<DynamicMethodInfo> Methods
    {
        get => _methods;
        set => this.RaiseAndSetIfChanged(ref _methods, value);
    }

    public DynamicMethodInfo? SelectedMethod
    {
        get => _selectedMethod;
        set => this.RaiseAndSetIfChanged(ref _selectedMethod, value);
    }

    public string ActionMessage
    {
        get => _actionMessage;
        set => this.RaiseAndSetIfChanged(ref _actionMessage, value);
    }

    public ReactiveCommand<Unit, Unit> LoadLibraryCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateInstanceCommand { get; }
    public ReactiveCommand<Unit, Unit> ExecuteMethodCommand { get; }

    public MainWindowViewModel()
    {
        _classes = new ObservableCollection<string>();
        _methods = new ObservableCollection<DynamicMethodInfo>();

        AvailableColors = new ObservableCollection<PieceColor>((PieceColor[])Enum.GetValues(typeof(PieceColor)));
        SelectedColor = AvailableColors.FirstOrDefault();

        LoadLibraryCommand = ReactiveCommand.Create(LoadLibrary);
        CreateInstanceCommand = ReactiveCommand.Create(CreatePieceInstance);
        ExecuteMethodCommand = ReactiveCommand.Create(ExecuteSelectedMethod);
    }

    private void LoadLibrary()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(LibraryPath) || !File.Exists(LibraryPath))
            {
                ActionMessage = "Library path is not set or file does not exist.";
                return;
            }

            var assembly = Assembly.LoadFrom(LibraryPath);
            Classes.Clear();
            SelectedClassName = null;
            _selectedClassType = null;
            _instance = null;
            Methods.Clear();
            SelectedMethod = null;

            var chessPieceBaseType = typeof(ChessPieceBase);

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && chessPieceBaseType.IsAssignableFrom(type))
                {
                    Classes.Add(type.FullName ?? type.Name);
                }
            }

            ActionMessage = Classes.Any()
                ? $"Library loaded. Found {Classes.Count} chess piece class(es)."
                : "No classes derived from ChessPieceBase found in the library.";
        }
        catch (Exception ex)
        {
            ActionMessage = $"Error loading library: {ex.Message}";
        }
    }

    private void UpdateSelectedClassType()
    {
        if (string.IsNullOrEmpty(_selectedClassName))
        {
            _selectedClassType = null;
            return;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(LibraryPath) || !File.Exists(LibraryPath))
            {
                ActionMessage = "Library not loaded. Please load the library first.";
                _selectedClassType = null;
                return;
            }
            var assembly = Assembly.LoadFrom(LibraryPath);
            _selectedClassType = assembly.GetType(_selectedClassName);
        }
        catch (Exception ex)
        {
            ActionMessage = $"Error selecting class: {ex.Message}";
            _selectedClassType = null;
        }
    }

    private void CreatePieceInstance()
    {
        if (_selectedClassType == null)
        {
            ActionMessage = "Please select a class type first.";
            return;
        }
        if (string.IsNullOrWhiteSpace(InitialPositionString))
        {
            ActionMessage = "Please enter an initial position (e.g., A1).";
            return;
        }

        if (!PositionParser.TryParse(InitialPositionString, out var parsedPosition))
        {
            ActionMessage = $"Invalid position format: {InitialPositionString}. Use chess notation (e.g., A1).";
            return;
        }

        try
        {
            _instance = Activator.CreateInstance(_selectedClassType, SelectedColor, parsedPosition);
            if (_instance is ChessPieceBase piece)
            {
                LoadMethodsForInstance();
                ActionMessage = $"{_selectedClassType.Name} created at {piece.Position.ToNotation()}. Select a method.";
            }
            else
            {
                ActionMessage = "Failed to create piece instance or instance is not a ChessPieceBase.";
                _instance = null;
            }
        }
        catch (Exception ex)
        {
            ActionMessage = $"Error creating instance: {(ex.InnerException?.Message ?? ex.Message)}";
            _instance = null;
        }
    }

    private void LoadMethodsForInstance()
    {
        Methods.Clear();
        SelectedMethod = null;
        if (_selectedClassType == null || _instance == null) return;

        var methods = _selectedClassType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.DeclaringType != typeof(object) &&
                          !m.IsSpecialName)
            .ToList();

        foreach (var method in methods.OrderBy(m => m.Name))
        {
            Methods.Add(new DynamicMethodInfo(method));
        }
    }

    private void ExecuteSelectedMethod()
    {
        if (_instance == null || SelectedMethod == null)
        {
            ActionMessage = "No instance or method selected.";
            return;
        }

        try
        {
            object[] methodParameters = Array.Empty<object>();
            if (SelectedMethod.Parameters.Any())
            {
                methodParameters = SelectedMethod.Parameters
                    .Select((p, i) =>
                    {
                        var valueString = SelectedMethod.ParameterViewModels[i].Value;
                        if (p.ParameterType == typeof((int, int)) ||
                            (p.ParameterType.IsGenericType &&
                             p.ParameterType.GetGenericTypeDefinition() == typeof(ValueTuple<,>) &&
                             p.ParameterType.GetGenericArguments()[0] == typeof(int) &&
                             p.ParameterType.GetGenericArguments()[1] == typeof(int)))
                        {
                            if (PositionParser.TryParse(valueString, out var parsedPos))
                            {
                                return (object)parsedPos;
                            }
                            throw new ArgumentException($"Invalid position notation: {valueString} for parameter {p.Name}. Use e.g. A1.");
                        }
                        if (p.ParameterType == typeof(string)) return (object)valueString;
                        if (p.ParameterType == typeof(int)) return (object)int.Parse(valueString);

                        throw new InvalidOperationException($"Unsupported parameter type: {p.ParameterType.Name} for parameter {p.Name}");
                    })
                    .ToArray();
            }

            var result = SelectedMethod.Method.Invoke(_instance, methodParameters);

            if (_instance is ChessPieceBase piece)
            {
                string pieceState = $"{_selectedClassType.Name} is at {piece.Position.ToNotation()}";
                if (result != null)
                {
                    ActionMessage = $"Method {SelectedMethod.Name} executed. Result: {result}. {pieceState}";
                }
                else
                {
                    ActionMessage = $"Method {SelectedMethod.Name} executed. {pieceState}";
                }
            }
            else
            {
                ActionMessage = $"Method {SelectedMethod.Name} executed.";
            }
        }
        catch (Exception ex)
        {
            ActionMessage = $"Error executing method {SelectedMethod.Name}: {(ex.InnerException?.Message ?? ex.Message)}";
        }
    }
}