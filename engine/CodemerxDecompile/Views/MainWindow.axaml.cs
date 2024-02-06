using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.TextMate;
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
            return new ReferenceVisualLineText(CurrentContext.VisualLine, segment.Length, segment.MemberDefinition, segment.MemberReference);
        }
    }
    
    private class ReferenceVisualLineText : VisualLineText
    {
        private static ReferenceVisualLineText? pressed;
        
        public ReferenceVisualLineText(VisualLine parentVisualLine, int length, IMemberDefinition? memberDefinition, MemberReference? memberReference)
            : base(parentVisualLine, length)
        {
            MemberDefinition = memberDefinition;
            MemberReference = memberReference;
        }

        public IMemberDefinition? MemberDefinition { get; }
        public MemberReference? MemberReference { get; }

        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            TextRunProperties.SetTextDecorations(TextDecorations.Underline);
            return base.CreateTextRun(startVisualColumn, context);
        }
        
        protected override void OnQueryCursor(PointerEventArgs e)
        {
            if (e.Handled)
            {
                base.OnQueryCursor(e);
                return;
            }

            if (e.Source is InputElement inputElement)
            {
                inputElement.Cursor = new Cursor(StandardCursorType.Hand);
            }
            
            base.OnQueryCursor(e);
        }
        
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (e.Handled)
            {
                base.OnPointerPressed(e);
                return;
            }
            
            var mainWindow = ((App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!.MainWindow as MainWindow)!;
            var textEditor = mainWindow.TextEditor;
            if (textEditor.TextArea.TextView.CapturePointer(e.Pointer))
            {
                pressed = this;
                e.Handled = true;
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
                
                if (MemberReference != null)
                {
                    viewModel.SelectNodeByMemberReference(MemberReference);
                }
                else if (MemberDefinition != null)
                {
                    viewModel.SelectNodeByMemberFullName(null, MemberDefinition.FullName);
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
    public IMemberDefinition? MemberDefinition { get; init; }
    public MemberReference? MemberReference { get; init; }
}
