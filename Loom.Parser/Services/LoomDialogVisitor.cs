using Loom.Parser.Generated;
using Loom.Parser.Models;

namespace Loom.Parser.Services;

public class LoomDialogVisitor : LoomParserBaseVisitor<object>
{
    public override FileNode VisitFile(LoomParser.FileContext context)
    {
        FileNode fileNode = new()
        {
            ParsedBlocks = context.block().Select(VisitBlock).ToList()
        };

        return fileNode;
    }

    public override BlockNode VisitBlock(LoomParser.BlockContext context)
    {
        var title = context.title().Text;
        var tags = context.tags()?.plainWords().Where(w => w.op.Type == LoomParser.WORD).Select(VisitPlainWords).ToList();
        var lines = context.line().Select(VisitLine).ToList();

        return new BlockNode
        {
            Title = title.GetText(),
            Tags = tags,
            Lines = lines
        };
    }

    public override ILineNode VisitLine(LoomParser.LineContext context)
    {
        if (context.jsif != null)
        {
            return VisitJsIfBlock(context.jsIfBlock());
        }

        if (context.dl != null)
        {
            return VisitDialogLine(context.dialogLine());
        }

        throw new NotSupportedException(context.GetText());
    }

    public override ILineNode VisitDialogLine(LoomParser.DialogLineContext context)
    {
        var indent = (context.indent?.Text ?? string.Empty).Length;
        var nameContext = context.name();
        var speaker = nameContext != null ? VisitName(nameContext) : null;
        
        var lineNode = new DialogLineNode
        {
            Indent = indent,
            Speaker = speaker,
            Fragments = context.lineContent().Select(VisitLineContent).ToList(),
        };

        return lineNode;
    }

    public override List<DialogLineNode.ILineNodeFragment> VisitName(LoomParser.NameContext context)
    {
        return context.lineContent().Select(VisitLineContent).ToList();
    }

    public override DialogLineNode.ILineNodeFragment VisitLineContent(LoomParser.LineContentContext context)
    {
        if (context.Text != null)
            return VisitTextFragment(context.textFragment());
        
        if (context.Script != null)
            return VisitJsBlock(context.jsBlock());

        if (context.Out != null)
            return VisitJsOutBlock(context.jsOutBlock());
        
        throw new NotSupportedException(context.GetText());
    }

    public override string VisitPlainWords(LoomParser.PlainWordsContext context)
    {
        return context.GetText();
    }

    public override DialogLineNode.ILineNodeFragment VisitTextFragment(LoomParser.TextFragmentContext context)
    {
        return new DialogLineNode.TextFragment
        {
            Text = context.GetText()
        };
    }

    public override DialogLineNode.ILineNodeFragment VisitJsBlock(LoomParser.JsBlockContext context)
    {
        return new DialogLineNode.ScriptFragment
        {
            Script = context.script.Text
        };
    }

    public override ILineNode VisitJsIfBlock(LoomParser.JsIfBlockContext context)
    {
        var indent = (context.indent?.Text ?? string.Empty).Length;
        return new JsIfNode
        {
            Indent = indent,
            Condition = context.condition.Text
        };
    }

    public override DialogLineNode.ILineNodeFragment VisitJsOutBlock(LoomParser.JsOutBlockContext context)
    {
        return new DialogLineNode.ScriptFragment
        {
            HasOutput = true,
            Script = context.script.Text
        };
    }
}