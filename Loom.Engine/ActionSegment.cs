namespace Loom.Engine;

public record ActionSegment(Action Action) : ISegment, ITemplate
{
    public string Name { get; set; } = string.Empty;
    public void Execute() => Action();
    public ISegment Evaluate()
    {
        return this;
    }
}