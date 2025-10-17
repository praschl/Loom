namespace Loom.Engine;

public abstract record Node
{
    public abstract void Activate(IDialogEvents events);
}

public record Line(string Text) : Node
{
    public override void Activate(IDialogEvents sharedEvents)
    {
        sharedEvents.OnLineReceived(this);
    }
}

public record Option(string Text);

public record OptionsList(params List<Option> Options) : Node
{
    public override void Activate(IDialogEvents sharedEvents)
    {
        sharedEvents.OnOptionsReceived(this);
    }
}

public record BlockNode : Node
{
    private int _nextNode;

    public List<Node> Children { get; } = [];

    public override void Activate(IDialogEvents events)
    {
        // can raise a `BlockStarted` event
    }

    public bool HasMoreContent => _nextNode < Children.Count;
    
    public Node GetNextNode()
    {
        if (_nextNode >= Children.Count)
        {
            throw new InvalidOperationException($"No more node available, check with {nameof(HasMoreContent)} before.");
        }

        return Children[_nextNode++];
    }
}