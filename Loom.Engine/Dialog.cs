namespace Loom.Engine;

public class DialogDefinition
{
    public List<Node> Nodes { get; } = [];

    public Dialog StartDialog()
    {
        return new Dialog(this);
    }
}

public interface IDialogEvents
{
    event Action<Line>? OnLine;
    void RaiseOnLine(Line line);
    
    event Action<OptionsList>? OnOptionsList;
    void RaiseOnOptionsList(OptionsList optionsList);

    event Action DialogFinished;
    void RaiseDialogFinished();
}

public class DialogEvents : IDialogEvents
{
    public event Action<Line>? OnLine;
    public void RaiseOnLine(Line line) => OnLine?.Invoke(line);
    
    public event Action<OptionsList>? OnOptionsList;
    public void RaiseOnOptionsList(OptionsList optionsList) => OnOptionsList?.Invoke(optionsList);
    
    public event Action? DialogFinished;
    public void RaiseDialogFinished() => DialogFinished?.Invoke();
}

public class Dialog
{
    private readonly DialogDefinition _definition;
    private int _nextNode;

    public Dialog(DialogDefinition definition, DialogEvents? events = null)
    {
        _definition = definition;
        DialogEvents = events ?? new DialogEvents();
    }

    public IDialogEvents DialogEvents { get; }
    
    public List<Node> Nodes => _definition.Nodes;

    public void Advance()
    {
        if (_nextNode >= Nodes.Count)
        {
            DialogEvents.RaiseDialogFinished();
            return;
        }
        
        var node = Nodes[_nextNode++];

        node.Activate(DialogEvents);
    }
}

public abstract record Node
{
    public abstract void Activate(IDialogEvents events);
}

public record Line(string Text) : Node
{
    public override void Activate(IDialogEvents sharedEvents)
    {
        sharedEvents.RaiseOnLine(this);
    }
}

public record Option(string Text);

public record OptionsList(params List<Option> Options) : Node
{
    public override void Activate(IDialogEvents sharedEvents)
    {
        sharedEvents.RaiseOnOptionsList(this);
    }
}