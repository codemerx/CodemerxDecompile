using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CodemerxDecompile.Nodes;

namespace CodemerxDecompile.ViewModels;

public interface IMainWindowViewModel
{
    ObservableCollection<AssemblyNode> AssemblyNodes { get; }
    IRelayCommand RemoveSelectedAssemblyCommand { get; }
}
