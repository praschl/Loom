namespace Loom.Engine;

public record ConditionalNode : INode, ITemplate
{
    public Func<bool> Condition { get; set; }

    public BlockNode WhenTrue { get; set; }
    public BlockNode WhenFalse { get; set; }

    public BlockNode GetCorrectNode() => Condition() ? WhenTrue : WhenFalse;
    
    public INode Evaluate()
    {
        return this;
    }
}