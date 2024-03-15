/*
    Copyright CodeMerx 2024
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

using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.TextMate;
using CodemerxDecompile.Extensions;
using CodemerxDecompile.Services;
using CodemerxDecompile.ViewModels;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using TextMateSharp.Grammars;

using static AvaloniaEdit.Utils.ExtensionMethods;

namespace CodemerxDecompile.Views;

public partial class MainWindow : Window
{
    internal static readonly TextSegmentCollection<ReferenceTextSegment> references = new();
    private static MainWindowViewModel viewModel;
    
    public MainWindow(ILogger<MainWindow> logger, MainWindowViewModel mainWindowViewModel,
        IAnalyticsService analyticsService, IAutoUpdateService autoUpdateService)
    {
        InitializeComponent();

        Program.UnhandledException += (_, e) =>
        {
            logger.LogCritical(e, "Unhandled exception occured");
        };
        
        viewModel = mainWindowViewModel;
        DataContext = viewModel;

        _ = analyticsService.TrackEventAsync(AnalyticsEvents.Startup);
        // Swallowing the exceptions on purpose to avoid problems in the auto-update taking down the entire app
        _ = autoUpdateService.CheckForNewerVersionAsync();
        
        TextEditor.TextArea.TextView.ElementGenerators.Add(new ReferenceElementGenerator());
        
        // TODO: Switch editor theme according with app theme
        var registryOptions = new RegistryOptions(ThemeName.LightPlus);
        var installation = TextEditor.InstallTextMate(registryOptions);
        // TODO: Set grammar according to selected language in the UI
        installation.SetGrammar(registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".cs").Id));

        TextEditor.KeyDown += (_, args) =>
        {
            if (args.Handled)
                return;

            if (args.Key != Key.F12)
                return;
            
            var reference = references.FindSegmentsContaining(TextEditor.CaretOffset).FirstOrDefault();
            if (reference == null)
                return;
            
            if (reference.Resolved)
            {
                viewModel.SelectNodeByMemberReference(reference.MemberReference);
            }
            else
            {
                viewModel.TryLoadUnresolvedReference(reference.MemberReference);
            }
                    
            args.Handled = true;
        };
        
        AddHandler(DragDrop.DragOverEvent, (_, args) =>
        {
            if (args.Handled)
                return;
            
            if (args.Data.Contains(DataFormats.Files))
            {
                args.DragEffects &= DragDropEffects.Copy;
            }
            else
            {
                args.DragEffects &= DragDropEffects.None;
            }
        });
        
        AddHandler(DragDrop.DragEnterEvent, (_, args) =>
        {
            if (args.Handled)
                return;
            
            if (args.Data.Contains(DataFormats.Files))
            {
                DragDropLabel.IsVisible = true;
            }
        });
        
        AddHandler(DragDrop.DragLeaveEvent, (_, args) =>
        {
            if (args.Handled)
                return;
            
            DragDropLabel.IsVisible = false;
        });
        
        AddHandler(DragDrop.DropEvent, (_, args) =>
        {
            if (args.Handled)
                return;
            
            DragDropLabel.IsVisible = false;
            
            if (!args.Data.Contains(DataFormats.Files))
                return;

            analyticsService.TrackEventAsync(AnalyticsEvents.OpenViaDragDrop);
            
            var files = args.Data.GetFiles()!;
            viewModel.LoadAssemblies(files.Select(file => file.Path.LocalPath));

            args.Handled = true;
        });
        
        PointerReleased += (_, args) =>
        {
            if (args.Handled)
                return;
            
            if (args.InitialPressMouseButton == MouseButton.XButton1 && viewModel.BackCommand.CanExecute(null))
            {
                viewModel.BackCommand.Execute(null);

                args.Handled = true;
            }
            else if (args.InitialPressMouseButton == MouseButton.XButton2 && viewModel.ForwardCommand.CanExecute(null))
            {
                viewModel.ForwardCommand.Execute(null);
                
                args.Handled = true;
            }
        };
    }
    
    private class ReferenceElementGenerator : VisualLineElementGenerator
    {
        public override int GetFirstInterestedOffset(int startOffset)
        {
            return references.FindFirstSegmentWithStartAfter(startOffset)?.StartOffset ?? -1;
        }

        public override VisualLineElement ConstructElement(int offset)
        {
            var segment = references.FindSegmentsContaining(offset).First();
            return new ReferenceVisualLineText(CurrentContext.VisualLine, segment.Length, segment.MemberReference, segment.Resolved);
        }
    }
    
    private class ReferenceVisualLineText : VisualLineText
    {
        private static readonly KeyModifiers NavigateToKeyModifier =
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? KeyModifiers.Meta : KeyModifiers.Control;
        private static ReferenceVisualLineText? pressed;
        private readonly MemberReference memberReference;
        private readonly bool resolved;
        
        public ReferenceVisualLineText(VisualLine parentVisualLine, int length, MemberReference memberReference, bool resolved)
            : base(parentVisualLine, length)
        {
            this.memberReference = memberReference;
            this.resolved = resolved;
        }
        
        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            if (!resolved)
            {
                TextRunProperties.SetForegroundBrush(Brushes.Red);
            }
            
            return base.CreateTextRun(startVisualColumn, context);
        }
        
        protected override void OnQueryCursor(PointerEventArgs e)
        {
            if (e.Handled)
            {
                base.OnQueryCursor(e);
                return;
            }

            if (e.Source is Control control)
            {
                if (e.KeyModifiers.HasFlag(NavigateToKeyModifier))
                {
                    control.Cursor = new Cursor(StandardCursorType.Hand);
                }
            }
            
            base.OnQueryCursor(e);
        }

        protected override void OnPointerEntered(PointerEventArgs e)
        {
            if (e.Handled)
            {
                base.OnQueryCursor(e);
                return;
            }
            
            if (e.Source is Control control)
            {
                if (!resolved)
                {
                    var assemblyName = (memberReference.GetTopDeclaringTypeOrSelf().Scope as AssemblyNameReference)?.FullName;
                    var message = memberReference switch
                    {
                        TypeReference => $"Ambiguous type reference. Generic parameters might be present. Please, locate the assembly where the type is defined.{System.Environment.NewLine}{System.Environment.NewLine}Assembly name: {assemblyName}",
                        _ => $"Ambiguous reference. Please, locate the assembly where the member is defined.{System.Environment.NewLine}{System.Environment.NewLine}Assembly name: {assemblyName}"
                    };
                    
                    ToolTip.SetPlacement(control, PlacementMode.Pointer);
                    ToolTip.SetTip(control, message);
                    ToolTip.SetIsOpen(control, true);
                }
            }
            
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            if (e.Handled)
            {
                base.OnPointerExited(e);
                return;
            }

            if (!resolved)
            {
                if (e.Source is Control control)
                {
                    ToolTip.SetTip(control, null);
                    ToolTip.SetIsOpen(control, false);
                }
            }
            
            base.OnPointerExited(e);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (e.Handled)
            {
                base.OnPointerPressed(e);
                return;
            }

            if (e.KeyModifiers.HasFlag(NavigateToKeyModifier))
            {
                var mainWindow = ((App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow)!;
                var textEditor = mainWindow.TextEditor;
                if (textEditor.TextArea.TextView.CapturePointer(e.Pointer))
                {
                    pressed = this;
                    e.Handled = true;
                }
            }

            base.OnPointerPressed(e);
        }

        protected override void OnPointerReleased(PointerEventArgs e)
        {
            if (e.Handled)
            {
                base.OnPointerReleased(e);
                return;
            }

            if (pressed == this)
            {
                var mainWindow = ((App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow)!;
                var textEditor = mainWindow.TextEditor;
                textEditor.TextArea.TextView.ReleasePointerCapture(e.Pointer);

                if (resolved)
                {
                    viewModel.SelectNodeByMemberReference(memberReference);
                }
                else
                {
                    viewModel.TryLoadUnresolvedReference(memberReference);
                }
                
                pressed = null;
                e.Handled = true;
            }

            base.OnPointerReleased(e);
        }
    }
}

public class ReferenceTextSegment : TextSegment
{
    public required MemberReference MemberReference { get; init; }
    public required bool Resolved { get; init; }
}
