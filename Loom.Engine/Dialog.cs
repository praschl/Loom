namespace Loom.Engine;

public class Dialog
{
    public List<Node> Nodes { get; } = [];

    public DialogRun StartDialog()
    {
        return new DialogRun(this);
    }
}