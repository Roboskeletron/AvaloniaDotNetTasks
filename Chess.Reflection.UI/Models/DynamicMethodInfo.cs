using Chess.Reflection.UI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chess.Reflection.UI.Models;

public class DynamicMethodInfo
{
    public MethodInfo Method { get; }
    public string Name => Method.Name;
    public ParameterInfo[] Parameters { get; }
    public List<ParameterViewModel> ParameterViewModels { get; }

    public DynamicMethodInfo(MethodInfo method)
    {
        Method = method;
        Parameters = method.GetParameters();
        ParameterViewModels = Parameters.Select((p, i) => new ParameterViewModel(p.Name ?? $"Param{i}")).ToList();
    }
}
