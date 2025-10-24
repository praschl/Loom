namespace Loom.Engine;

public record ActionNode(Action Action) : INode, ITemplate
{
    public string Name { get; set; } = string.Empty;
    public void Execute() => Action();
    public INode Evaluate()
    {
        return this;
    }
}