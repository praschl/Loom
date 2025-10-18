namespace Loom.Engine;

public class DialogRun
{
    private Node? _currentNode;
    private BlockNode _currentBlock;
    private readonly Stack<BlockNode> _blockNodes = [];
    private bool _finished;

    public DialogRun(BlockNode rootNode, DialogEvents? events = null)
    {
        _currentBlock = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
        DialogEvents = events ?? new DialogEvents();
    }

    public IDialogEvents DialogEvents { get; }

    public void Advance()
    {
        AssertNotFinished();

        if (_currentNode is not null and not Line)
        {
            throw new InvalidOperationException($"Cannot use advance on {_currentNode.GetType()}");
        }

        ActivateNextNode();
    }

    public void SelectOption(int option)
    {
        AssertNotFinished();

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
        while (true)
        {
            var newNode = ActivateNextNode2();
            if (newNode is null or ContentNode)
            {
                break;
            }
        }
    }

    private Node? ActivateNextNode2()
    {
        if (!_currentBlock.HasMoreContent)
        {
            if (_currentNode is null)
            {
                // just in case the root block was empty
                DialogEvents.OnDialogStarted();
            }

            if (!_blockNodes.TryPop(out var parentBlock))
            {
                DialogEvents.OnDialogFinished();
                _finished = true;
                return null;
            }

            _currentBlock = parentBlock;
            return ActivateNextNode2();
        }

        bool started = _currentNode is null;
        _currentNode = _currentBlock.GetNextNode();

        if (started)
        {
            DialogEvents.OnDialogStarted();
        }

        if (_currentNode is ContentNode contentNode)
            contentNode.PushContent(DialogEvents);

        if (_currentNode is BlockNode blockNode)
        {
            _blockNodes.Push(_currentBlock);
            _currentBlock = blockNode;
        }

        return _currentNode;
    }

    private void AssertNotFinished()
    {
        if (_finished)
            throw new InvalidOperationException("Dialog is already finished");
    }
}