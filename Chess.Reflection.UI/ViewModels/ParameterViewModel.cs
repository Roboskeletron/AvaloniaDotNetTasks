using ReactiveUI;

namespace Chess.Reflection.UI.ViewModels;

public class ParameterViewModel : ViewModelBase
{
    private string _value = "";

    public string Name { get; }
    public string Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    public ParameterViewModel(string name)
    {
        Name = name;
    }
}
