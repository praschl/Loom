namespace Loom.Engine;

public abstract record Node
{
}

public abstract record ContentNode : Node
{
    public abstract void PushContent(IDialogEvents events);
}

public record Line(string Text) : ContentNode
{
    public override void PushContent(IDialogEvents sharedEvents)
    {
        sharedEvents.OnLineReceived(this);
    }
}

public record Option(string Text);

public record OptionsList(params List<Option> Options) : ContentNode
{
    public override void PushContent(IDialogEvents sharedEvents)
    {
        sharedEvents.OnOptionsReceived(this);
    }
}

public record BlockNode(string Name) : Node
{
    private int _nextNode;

    public List<Node> Children { get; } = [];

    public bool HasMoreContent => _nextNode < Children.Count;

    public Node GetNextNode()
    {
        if (_nextNode >= Children.Count)
        {
            throw new InvalidOperationException($"No more node available, check with {nameof(HasMoreContent)} before.");
        }

        return Children[_nextNode++];
    }

    public void Starting(IDialogEvents dialogEvents)
    {
        dialogEvents.OnBlockStarted(this);
    }

    public void Finishing(IDialogEvents dialogEvents)
    {
        dialogEvents.OnBlockFinishing(this);
    }
}

public record ConditionalNode : Node
{
    public Func<bool> Condition { get; set; }

    public BlockNode WhenTrue { get; set; }
    public BlockNode WhenFalse { get; set; }

    public BlockNode GetCorrectNode() => Condition() ? WhenTrue : WhenFalse;
}

public record ActionNode(Action Action) : Node
{
    public string Name { get; set; }
    public void Execute() => Action();
}

// TODO:
// set variable = Action // parameterless, what's happening is done in the Parser, the parser just creates the Action
//   Conditions in Blocks or Options are also just a parameterless Func<bool>, same here, it will all be handled in the Parser
// Commands also are just actions
// Variables in Text -> Textfragments, lazy formatting
// Options with condition
// multiple named Blocknode in Dialog
// goto Blocknode by name
// gosub Blocknode by name
// tags for lines & options

// design decision: "set variable", "commands" are both parameterless Action. whats happening is determined in den Parser that creates the Action
// same goes for functions that return something or conditions - they are just a Func<Value> or Func<bool> respectively

// design decision: no async here
// because when for example we encounter the command "open inventory", the following will happen
// - command is executed
// - unity handler can now do somethingn like this
//   await EventBus.SendAsync("Open Inventory")
//   await inventory.Closed // not sure I need this
//   dialogRunner.Advance()
// - command was executed without knowing about async stuff, and immediately continues to first real content, displaying "here are my wares"
// - only when the inventory closed, the next line will appear

public readonly record struct LoomValue
{
    public enum LoomType
    {
        String,
        Number, // (use decimal)
        Boolean,
    }
    
    public LoomType Type { get; init; }

    public string StringValue { get; init; }
    public decimal NumberValue { get; init; }
    public bool BooleanValue { get; init; }
}