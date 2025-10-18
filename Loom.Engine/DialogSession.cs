namespace Loom.Engine;

public class DialogSession
{
    private Node? _currentNode;
    private BlockNode _currentBlock;
    private readonly Stack<BlockNode> _blockNodes = [];
    private bool _finished;

    public DialogSession(BlockNode rootNode, DialogEvents? events = null)
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

        Continue();
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

        Continue();
    }

    private void Continue()
    {
        while (true)
        {
            var node = ActivateNextNode();

            // the new node could be a function call or setting a variable
            // in this case we have already executed it, but we didn't get new content for display

            if (node is null or ContentNode)
            {
                break;
            }
        }
    }

    private Node? ActivateNextNode()
    {
        while (true)
        {
            if (!_currentBlock.HasMoreContent)
            {
                if (!HandleExhaustedBlock())
                    return null;

                continue;
            }

            bool isFirstNode = _currentNode is null;
            _currentNode = _currentBlock.GetNextNode();

            if (isFirstNode)
            {
                DialogEvents.OnDialogStarted();
                _currentBlock.Starting(DialogEvents);
            }

            if (HandleCurrentNode())
                continue;

            return _currentNode;
        }
    }

    private bool HandleExhaustedBlock()
    {
        if (_currentNode is null)
        {
            // Root block was empty
            DialogEvents.OnDialogStarted();
            _currentBlock.Starting(DialogEvents);
        }

        _currentBlock.Finishing(DialogEvents);

        if (!_blockNodes.TryPop(out var parentBlock))
        {
            // No more blocks → dialog finished
            DialogEvents.OnDialogFinished();
            _finished = true;
            return false;
        }

        _currentBlock = parentBlock;
        return true;
    }

    private bool HandleCurrentNode()
    {
        switch (_currentNode)
        {
            case ContentNode contentNode:
                contentNode.PushContent(DialogEvents);
                return false;

            case BlockNode blockNode:
                Activate(blockNode);
                return true;

            case ConditionalNode conditional:
                Activate(conditional.GetCorrectNode());
                return true;

            default:
                return false;
        }

        void Activate(BlockNode blockNode)
        {
            _blockNodes.Push(_currentBlock);
            _currentBlock = blockNode;
            _currentBlock.Starting(DialogEvents);
        }
    }

    private void AssertNotFinished()
    {
        if (_finished)
            throw new InvalidOperationException("Dialog is already finished");
    }
}