namespace Loom.Engine;

public record BlockNode(string Name) : INode, ITemplate
{
    private int _nextNode;

    public List<ITemplate> Children { get; } = [];

    public bool HasMoreContent => _nextNode < Children.Count;

    public INode GetNextNode()
    {
        if (_nextNode >= Children.Count)
        {
            throw new InvalidOperationException($"No more node available, check with {nameof(HasMoreContent)} before.");
        }

        var node = Children[_nextNode++];
        return node.Evaluate();
    }

    public void Starting(IDialogEvents dialogEvents)
    {
        dialogEvents.OnBlockStarted(this);
    }

    public void Finishing(IDialogEvents dialogEvents)
    {
        dialogEvents.OnBlockFinishing(this);
    }

    public INode Evaluate()
    {
        return this;
    }
}