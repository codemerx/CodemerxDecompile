using System.Linq;
using Avalonia.Controls;
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

        TextEditor.Document = new TextDocument();
        TextEditor.TextArea.TextView.ElementGenerators.Add(new ReferenceElementGenerator());
        
        // TODO: Switch editor theme according with app theme
        var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        var installation = TextEditor.InstallTextMate(registryOptions);
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
        public ReferenceVisualLineText(VisualLine parentVisualLine, int length, IMemberDefinition? memberDefinition, MemberReference? memberReference)
            : base(parentVisualLine, length)
        {
            MemberDefinition = memberDefinition;
            MemberReference = memberReference;
        }

        public IMemberDefinition? MemberDefinition { get; }
        public MemberReference? MemberReference { get; }

        // For debugging purposes
        // public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        // {
        //     this.TextRunProperties.SetForegroundBrush(context.TextView.LinkTextForegroundBrush);
        //     this.TextRunProperties.SetBackgroundBrush(context.TextView.LinkTextBackgroundBrush);
        //     this.TextRunProperties.SetTextDecorations(TextDecorations.Underline);
        //     return base.CreateTextRun(startVisualColumn, context);
        // }
        
        // Not working for now
        // protected override void OnQueryCursor(PointerEventArgs e)
        // {
        //     if (e.Source is InputElement inputElement)
        //     {
        //         inputElement.Cursor = new Cursor(StandardCursorType.Hand);
        //     }
        //     
        //     e.Handled = true;
        // }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (MemberReference != null)
            {
                viewModel.SelectNodeByMemberReference(MemberReference);
            }
            else if (MemberDefinition != null)
            {
                viewModel.SelectNodeByMemberFullName(null, MemberDefinition.FullName);
            }
        }
    }
}

public class ReferenceTextSegment : TextSegment
{
    public IMemberDefinition? MemberDefinition { get; init; }
    public MemberReference? MemberReference { get; init; }
}
