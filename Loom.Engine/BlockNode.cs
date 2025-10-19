namespace Loom.Engine;

public record BlockNode(string Name) : Node
{
    private int _nextNode;

    public List<Node> Children { get; } = [];

    public bool HasMoreContent => _nextNode < Children.Count;

    public Node GetNextNode()
    {
        if (_nextNode >= Children.Count)
        {
            throw new InvalidOperationException($"No more node available, check with {nameof(HasMoreContent)} before.");
        }

        var node = Children[_nextNode++];
        if (node is IEvaluateable evaluateable)
            node = evaluateable.Evaluate();

        return node;
    }

    public void Starting(IDialogEvents dialogEvents)
    {
        dialogEvents.OnBlockStarted(this);
    }

    public void Finishing(IDialogEvents dialogEvents)
    {
        dialogEvents.OnBlockFinishing(this);
    }
}