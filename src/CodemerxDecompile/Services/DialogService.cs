/*
    Copyright 2024 CodeMerx
    This file is part of CodemerxDecompile.

    CodemerxDecompile is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    CodemerxDecompile is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.
*/

using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;

namespace CodemerxDecompile.Services;

public class DialogService : IDialogService
{
    public void ShowDialog<TWindow>()
        where TWindow : Window
    {
        var lifetime = App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var window = App.Current.Services.GetRequiredService<TWindow>();
        window.ShowDialog(lifetime!.MainWindow!);
    }
}
