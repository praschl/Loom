namespace Loom.Engine;

public class DialogSession
{
    private ISegment? _currentSegment;
    private BlockSegment _currentBlock;
    private readonly Stack<BlockSegment> _blockSegments = [];
    private bool _finished;

    public DialogSession(BlockSegment rootSegment, DialogEvents? events = null)
    {
        _currentBlock = rootSegment ?? throw new ArgumentNullException(nameof(rootSegment));
        DialogEvents = events ?? new DialogEvents();
    }

    public IDialogEvents DialogEvents { get; }

    public void Advance()
    {
        AssertNotFinished();

        if (_currentSegment is not null and not Line)
        {
            throw new InvalidOperationException($"Cannot use advance on {_currentSegment.GetType()}");
        }

        Continue();
    }

    public void SelectOption(int option)
    {
        AssertNotFinished();

        if (_currentSegment is null)
        {
            throw new InvalidOperationException($"Cannot use SelectOption when dialog hasn't started");
        }

        if (_currentSegment is not OptionsList)
        {
            throw new InvalidOperationException($"Cannot use SelectOption on {_currentSegment.GetType()}");
        }

        // set variables or select next segment group or whatever here

        Continue();
    }

    private void Continue()
    {
        while (true)
        {
            var segment = ActivateNextSegment();

            // the new segment could be a function call or setting a variable
            // in this case we have already executed it, but we didn't get new content for display

            if (segment is null or ContentSegment)
            {
                break;
            }
        }
    }

    private ISegment? ActivateNextSegment()
    {
        while (true)
        {
            if (!_currentBlock.HasMoreContent)
            {
                if (!HandleExhaustedBlock())
                    return null;

                continue;
            }

            bool isFirstsegment = _currentSegment is null;
            _currentSegment = _currentBlock.GetNextSegment();

            if (isFirstsegment)
            {
                DialogEvents.OnDialogStarted();
                _currentBlock.Starting(DialogEvents);
            }

            if (HandleCurrentSegment())
                continue;

            return _currentSegment;
        }
    }

    private bool HandleExhaustedBlock()
    {
        if (_currentSegment is null)
        {
            // Root block was empty
            DialogEvents.OnDialogStarted();
            _currentBlock.Starting(DialogEvents);
        }

        _currentBlock.Finishing(DialogEvents);

        if (!_blockSegments.TryPop(out var parentBlock))
        {
            // No more blocks → dialog finished
            DialogEvents.OnDialogFinished();
            _finished = true;
            return false;
        }

        _currentBlock = parentBlock;
        return true;
    }

    private bool HandleCurrentSegment()
    {
        switch (_currentSegment)
        {
            case null:
                return true; 
            
            case ContentSegment contentSegment:
                contentSegment.PushContent(DialogEvents);
                return false;

            case BlockSegment blockSegment:
                Activate(blockSegment);
                return true;

            case ConditionalSegment conditional:
                var segment = conditional.GetCorrectSegment();
                if (segment is not null)
                    Activate(segment);
                return true;

            case ActionSegment action:
                DialogEvents.OnLog($"Starting action {action.Name}", action);
                action.Execute();
                DialogEvents.OnLog($"Finished action {action.Name}", action);
                return true;
            
            default:
                throw new NotSupportedException($"{_currentSegment.GetType()} is not supported.");
        }

        void Activate(BlockSegment blockSegment)
        {
            _blockSegments.Push(_currentBlock);
            _currentBlock = blockSegment;
            _currentBlock.Starting(DialogEvents);
        }
    }

    private void AssertNotFinished()
    {
        if (_finished)
            throw new InvalidOperationException("Dialog is already finished");
    }
}