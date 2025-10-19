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

// TODO:
// set variable = Action // parameterlos, was wie wo, wird noch beim Parser geregelt, der nur noch die Action herstellt
//   auch bei conditional block oder option with condition ist das nur 1 parameterlose Func<bool>, weil das was wie wo auch dort im Parser geregelt wird
// commands also are just actions
// Variables in Text -> Textfragments, lazy formatting
// Options with condition
// multiple named Blocknode in Dialog
// goto Blocknode by name
// gosub Blocknode by name
// tags for lines & options

// design decision: no async here
// because when for example we encounter the command "open inventory", the following will happen
// - command is executed
// - unity handler can now do somethingn like this
//   await EventBus.SendAsync("Open Inventory")
//   await inventory.Closed // not sure I need this
//   dialogRunner.Advance()
// - command was executed without knowing about async stuff, and immediately continues to first real content, displaying "here are my wares"
// - only when the inventory closed, the next line will appear
