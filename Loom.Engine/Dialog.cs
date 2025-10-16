namespace Loom.Engine;

public class DialogDefinition
{
    public List<Line> Lines { get; } = [];

    public Dialog StartDialog()
    {
        return new Dialog { Lines = Lines.ToList() };
    }
}

public class Dialog
{
    public event Action<Line>? OnLine;
    
    private int _nextLine = 0;
    
    public List<Line> Lines { get; internal init; } = [];

    public void Advance()
    {
        if (_nextLine >= Lines.Count) return;
        
        var line = Lines[_nextLine++];
        OnLine?.Invoke(line);
    }
}

public record Line(string Text);