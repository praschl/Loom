namespace Loom.Engine;

public record BlockSegment(string Name) : ISegment, ITemplate
{
    private int _nextSegment;

    public List<ITemplate> Children { get; } = [];

    public bool HasMoreContent => _nextSegment < Children.Count;

    public ISegment GetNextSegment()
    {
        if (_nextSegment >= Children.Count)
        {
            throw new InvalidOperationException($"No more segments available, check with {nameof(HasMoreContent)} before.");
        }

        var segment = Children[_nextSegment++];
        return segment.Evaluate();
    }

    public void Starting(IDialogEvents dialogEvents)
    {
        dialogEvents.OnBlockStarted(this);
    }

    public void Finishing(IDialogEvents dialogEvents)
    {
        dialogEvents.OnBlockFinishing(this);
    }

    public ISegment Evaluate()
    {
        return this;
    }
}