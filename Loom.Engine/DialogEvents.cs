namespace Loom.Engine;

public interface IDialogEvents
{
    event Action DialogStarted;
    void OnDialogStarted();

    event Action<BlockNode> BlockStarted;
    void OnBlockStarted(BlockNode blockNode);
    
    event Action<Line>? LineReceived;
    void OnLineReceived(Line line);
    
    event Action<OptionsList>? OptionsReceived;
    void OnOptionsReceived(OptionsList optionsList);
    
    event Action<BlockNode> BlockFinishing;
    void OnBlockFinishing(BlockNode blockNode);

    event Action DialogFinished;
    void OnDialogFinished();
}

public class DialogEvents : IDialogEvents
{
    public event Action? DialogStarted;
    public void OnDialogStarted() => DialogStarted?.Invoke();
    
    public event Action<BlockNode>? BlockStarted;
    public void OnBlockStarted(BlockNode blockNode) => BlockStarted?.Invoke(blockNode);

    public event Action<Line>? LineReceived;
    public void OnLineReceived(Line line) => LineReceived?.Invoke(line);
    
    public event Action<OptionsList>? OptionsReceived;
    public void OnOptionsReceived(OptionsList optionsList) => OptionsReceived?.Invoke(optionsList);

    public event Action<BlockNode>? BlockFinishing;
    public void OnBlockFinishing(BlockNode blockNode) => BlockFinishing?.Invoke(blockNode);

    public event Action? DialogFinished;
    public void OnDialogFinished() => DialogFinished?.Invoke();
}
