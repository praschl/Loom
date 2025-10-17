namespace Loom.Engine;

public class DialogRun
{
    private readonly Dialog _definition;
    private int _nextNode;

    public DialogRun(Dialog definition, DialogEvents? events = null)
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
            DialogEvents.OnDialogFinished();
            return;
        }
        
        var node = Nodes[_nextNode++];

        node.Activate(DialogEvents);
    }
}