namespace Loom.Engine;

public record ConditionalNode : INode, ITemplate
{
    public required Func<bool> Condition { get; set; }

    public required BlockNode WhenTrue { get; set; }
    public BlockNode? WhenFalse { get; set; }

    public BlockNode? GetCorrectNode()
    {
        if (Condition())
            return WhenTrue;

        return WhenFalse;
    }

    public INode Evaluate()
    {
        return this;
    }
}