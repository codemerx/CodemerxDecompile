using System;
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
using CodemerxDecompile.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Mono.Cecil;
using TextMateSharp.Grammars;

using static AvaloniaEdit.Utils.ExtensionMethods;

namespace CodemerxDecompile.Views;

public partial class MainWindow : Window
{
    internal static readonly TextSegmentCollection<ReferenceTextSegment> references = new();
    private static MainWindowViewModel viewModel;
    
    public MainWindow()
    {
        InitializeComponent();
        viewModel = App.Current.Services.GetService<MainWindowViewModel>()!;
        DataContext = viewModel;

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
                        TypeReference => $"Ambiguous type reference. Generic parameters might be present. Please, locate the assembly where the type is defined.{Environment.NewLine}{Environment.NewLine}Assembly name: {assemblyName}",
                        _ => $"Ambiguous reference. Please, locate the assembly where the member is defined.{Environment.NewLine}{Environment.NewLine}Assembly name: {assemblyName}"
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
