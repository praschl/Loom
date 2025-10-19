namespace Loom.Engine;

public interface IEvaluateable
{
    Node Evaluate();
}

public abstract record Node
{
}

public abstract record ContentNode : Node
{
    public abstract void PushContent(IDialogEvents events);
}

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