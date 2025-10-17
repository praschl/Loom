namespace Loom.Engine;

public class DialogRun
{
    private Node? _currentNode;
    private BlockNode _currentBlock;

    public DialogRun(BlockNode rootNode, DialogEvents? events = null)
    {
        _currentBlock = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
        DialogEvents = events ?? new DialogEvents();
    }

    public IDialogEvents DialogEvents { get; }

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
        if (_currentNode is null)
        {
            throw new InvalidOperationException($"Cannot use SelectOption when dialog hasn't started");
        }

        if (_currentNode is not OptionsList)
        {
            throw new InvalidOperationException($"Cannot use SelectOption on {_currentNode.GetType()}");
        }

        // set variables or select next node group or whatever here
        
        ActivateNextNode();
    }

    private void ActivateNextNode()
    {
        if (!_currentBlock.HasMoreContent)
        {
            DialogEvents.OnDialogFinished();
            return;
        }

        bool started = _currentNode is null;
        _currentNode = _currentBlock.GetNextNode();

        if (started)
        {
            DialogEvents.OnDialogStarted();
        }
        
        _currentNode.Activate(DialogEvents);
    }
}
