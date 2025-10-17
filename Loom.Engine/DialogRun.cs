namespace Loom.Engine;

public class DialogRun
{
    private readonly Dialog _definition;
    private int _nextNode;

    private Node _currentNode;

    public DialogRun(Dialog definition, DialogEvents? events = null)
    {
        _definition = definition;
        DialogEvents = events ?? new DialogEvents();
    }

    public IDialogEvents DialogEvents { get; }

    public List<Node> Nodes => _definition.Nodes;

    public void Advance()
    {
        if (_currentNode is not null and not Line)
        {
            throw new InvalidOperationException($"Cannot use advance on {_currentNode.GetType()}");
        }

        ActivateNextNode();
    }

    public void SelectOption(int option)
    {
        if (_currentNode is not null and not OptionsList)
        {
            throw new InvalidOperationException($"Cannot use SelectOption on {_currentNode.GetType()}");
        }

        // set variables or select next node group or whatever here
        
        ActivateNextNode();
    }

    private void ActivateNextNode()
    {
        if (_nextNode >= Nodes.Count)
        {
            DialogEvents.OnDialogFinished();
            return;
        }

        _currentNode = Nodes[_nextNode++];

        _currentNode.Activate(DialogEvents);
    }
}