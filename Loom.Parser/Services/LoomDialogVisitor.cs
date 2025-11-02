using Generated;
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
        var line = context.dialogLine();
        if (line != null)
            return VisitDialogLine(line);

        throw new NotSupportedException(context.ToString());
    }

    public override LineNode VisitDialogLine(LoomParser.DialogLineContext context)
    {
        var sentences = context.textFragment().Select(VisitTextFragment).ToList();

        var lineNode = new LineNode
        {
            Speaker = context.name?.Text,
            Fragments = sentences,
        };
        
        return lineNode;
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
}