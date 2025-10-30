namespace Loom.Engine;

public interface IDialogEvents
{
    event Action DialogStarted;
    void OnDialogStarted();

    event Action<BlockSegment> BlockStarted;
    void OnBlockStarted(BlockSegment blockSegment);
    
    event Action<Line>? LineReceived;
    void OnLineReceived(Line line);
    
    event Action<OptionsList>? OptionsReceived;
    void OnOptionsReceived(OptionsList optionsList);
    
    event Action<BlockSegment> BlockFinishing;
    void OnBlockFinishing(BlockSegment blockSegment);

    event Action DialogFinished;
    void OnDialogFinished();

    event Action<string, ISegment> Log;
    void OnLog(string text, ISegment segment);
}

public class DialogEvents : IDialogEvents
{
    public event Action? DialogStarted;
    public void OnDialogStarted() => DialogStarted?.Invoke();
    
    public event Action<BlockSegment>? BlockStarted;
    public void OnBlockStarted(BlockSegment blockSegment) => BlockStarted?.Invoke(blockSegment);

    public event Action<Line>? LineReceived;
    public void OnLineReceived(Line line) => LineReceived?.Invoke(line);
    
    public event Action<OptionsList>? OptionsReceived;
    public void OnOptionsReceived(OptionsList optionsList) => OptionsReceived?.Invoke(optionsList);

    public event Action<BlockSegment>? BlockFinishing;
    public void OnBlockFinishing(BlockSegment blockSegment) => BlockFinishing?.Invoke(blockSegment);

    public event Action? DialogFinished;
    public void OnDialogFinished() => DialogFinished?.Invoke();
    public event Action<string, ISegment>? Log;
    public void OnLog(string text, ISegment segment) => Log?.Invoke(text, segment);
}
