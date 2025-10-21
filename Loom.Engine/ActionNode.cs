namespace Loom.Engine;

public record ActionNode(Action Action) : INode, ITemplate
{
    public string Name { get; set; }
    public void Execute() => Action();
    public INode Evaluate()
    {
        return this;
    }
}