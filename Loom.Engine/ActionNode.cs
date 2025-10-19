namespace Loom.Engine;

public record ActionNode(Action Action) : Node
{
    public string Name { get; set; }
    public void Execute() => Action();
}