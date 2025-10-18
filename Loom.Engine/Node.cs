namespace Loom.Engine;

public abstract record Node
{
}

public abstract record ContentNode : Node
{
    public abstract void PushContent(IDialogEvents events);
}

public record Line(string Text) : ContentNode
{
    public override void PushContent(IDialogEvents sharedEvents)
    {
        sharedEvents.OnLineReceived(this);
    }
}

public record Option(string Text);

public record OptionsList(params List<Option> Options) : ContentNode
{
    public override void PushContent(IDialogEvents sharedEvents)
    {
        sharedEvents.OnOptionsReceived(this);
    }
}

public record BlockNode : Node
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

        return Children[_nextNode++];
    }
}

public record ConditionalNode : Node
{
    public Func<bool> Condition { get; set; }
    
    public BlockNode WhenTrue { get; set; }
    public BlockNode WhenFalse { get; set; }

    public BlockNode GetCorrectNode() => Condition() ? WhenTrue : WhenFalse;
}
