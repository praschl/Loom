namespace Loom.Engine;

public abstract record Node
{
    public abstract void Activate(IDialogEvents events);
}

public record Line(string Text) : Node
{
    public override void Activate(IDialogEvents sharedEvents)
    {
        sharedEvents.OnLineReceived(this);
    }
}

public record Option(string Text);

public record OptionsList(params List<Option> Options) : Node
{
    public override void Activate(IDialogEvents sharedEvents)
    {
        sharedEvents.OnOptionsReceived(this);
    }
}
