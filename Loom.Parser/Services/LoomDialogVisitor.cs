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

    public override LineNode VisitLine(LoomParser.LineContext context)
    {
        var nameContext = context.name();
        var speaker = nameContext != null ? VisitName(nameContext) : null;
        
        var lineNode = new LineNode
        {
            Speaker = speaker,
            Fragments = context.lineContent().Select(VisitLineContent).ToList(),
        };

        return lineNode;
    }

    public override List<LineNode.ILineNodeFragment> VisitName(LoomParser.NameContext context)
    {
        return context.lineContent().Select(VisitLineContent).ToList();
    }

    public override LineNode.ILineNodeFragment VisitLineContent(LoomParser.LineContentContext context)
    {
        if (context.Text != null)
            return VisitTextFragment(context.textFragment());
        
        if (context.Script != null)
            return VisitScriptBlock(context.scriptBlock());

        throw new NotSupportedException(context.GetText());
    }

    public override string VisitPlainWords(LoomParser.PlainWordsContext context)
    {
        return context.GetText();
    }

    public override LineNode.ILineNodeFragment VisitTextFragment(LoomParser.TextFragmentContext context)
    {
        return new LineNode.TextFragment
        {
            Text = context.GetText()
        };
    }

    //

    public override LineNode.ILineNodeFragment VisitScriptBlock(LoomParser.ScriptBlockContext context)
    {
        return new LineNode.ScriptFragment
        {
            Script = context.GetText()
        };
    }
}