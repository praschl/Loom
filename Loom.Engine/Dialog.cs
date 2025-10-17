namespace Loom.Engine;

public class DialogDefinition
{
    public List<Line> Lines { get; } = [];

    public Dialog StartDialog()
    {
        return new Dialog(this);
    }
}

public interface ISendDialogEvents
{
    event Action<Line>? OnLine;
    void RaiseOnLine(Line line);
}

public class DialogEvents : ISendDialogEvents
{
    public event Action<Line>? OnLine;
    public void RaiseOnLine(Line line) => OnLine?.Invoke(line);
}

public class Dialog
{
    private readonly DialogDefinition _definition;
    private int _nextLine = 0;

    public Dialog(DialogDefinition definition)
    {
        _definition = definition;
    }

    public ISendDialogEvents DialogEvents { get; } = new DialogEvents();
    
    public List<Line> Lines => _definition.Lines;

    public void Advance()
    {
        if (_nextLine >= Lines.Count) return;
        
        var line = Lines[_nextLine++];

        line.RaiseEvent(DialogEvents);
    }
}

public record Line(string Text)
{
    public void RaiseEvent(ISendDialogEvents sharedEvents)
    {
        sharedEvents.RaiseOnLine(this);
    }
}