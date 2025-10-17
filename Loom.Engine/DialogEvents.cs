namespace Loom.Engine;

public interface IDialogEvents
{
    event Action<Line>? LineReceived;
    void OnLineReceived(Line line);
    
    event Action<OptionsList>? OptionsReceived;
    void OnOptionsReceived(OptionsList optionsList);

    event Action DialogFinished;
    void OnDialogFinished();
}

public class DialogEvents : IDialogEvents
{
    public event Action<Line>? LineReceived;
    public void OnLineReceived(Line line) => LineReceived?.Invoke(line);
    
    public event Action<OptionsList>? OptionsReceived;
    public void OnOptionsReceived(OptionsList optionsList) => OptionsReceived?.Invoke(optionsList);
    
    public event Action? DialogFinished;
    public void OnDialogFinished() => DialogFinished?.Invoke();
}
