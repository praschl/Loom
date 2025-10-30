using Loom.Parser.Models;

namespace Loom.Parser.Services;

public class LoomDialogVisitor : LoomBaseVisitor<object>
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
        var tags = context.tags();
        var lines = context.line();

        return new BlockNode
        {
            Title = title.GetText(),
            Tags = tags?.plainWords().Where(w=>w.op.Type == LoomParser.WORD).Select(VisitPlainWords).ToList()
        };
    }

    public override string VisitPlainWords(LoomParser.PlainWordsContext context)
    {
        return context.GetText();
    }
}
