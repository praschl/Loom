using Antlr4.Runtime.Tree;

namespace Loom.Parser.Services;

public class ParsedFile
{
    public List<ParsedBlock>? ParsedBlocks { get; set; }
}

public class ParsedBlock
{
    public string? Title { get; set; }
    public List<string>? Tags { get; set; }
}

public class LoomDialogVisitor : LoomBaseVisitor<object>
{
    public override ParsedFile Visit(IParseTree tree)
    {
        return (ParsedFile)base.Visit(tree);
    }

    public override ParsedFile VisitFile(LoomParser.FileContext context)
    {
        ParsedFile file = new()
        {
            ParsedBlocks = context.block().Select(VisitBlock).ToList()
        };

        return file;
    }

    public override ParsedBlock VisitBlock(LoomParser.BlockContext context)
    {
        var title = context.title().Text;
        var tags = context.tags();
        var lines = context.line();

        return new ParsedBlock
        {
            Title = title.GetText(),
            Tags = tags?.words().Select(t => t.GetText()).ToList(),
        };
    }
}
