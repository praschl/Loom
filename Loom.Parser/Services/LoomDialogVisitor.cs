namespace Loom.Parser.Services;

public class LoomDialogVisitor : LoomBaseVisitor<object>
{
    public override object VisitFile(LoomParser.FileContext context)
    {
        return base.VisitFile(context);
    }

    public override object VisitBlock(LoomParser.BlockContext context)
    {
        var title = context.title().Text;
        var tags = context.tags().words();
        var lines = context.line();
        
        return base.VisitBlock(context);
    }
}
