using System.Diagnostics;
using System.Globalization;

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

// design decision: no async here for events and Func<T>s
// because when for example we encounter the command "open inventory", the following will happen
// - command is executed
// - unity handler can now do something like this
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
    
    public LoomType Type { get; }

    public string StringValue { get; }
    public decimal NumberValue { get; }
    public bool BooleanValue { get; }

    public override string ToString()
    {
        return Type switch
        {
            LoomType.String => StringValue,
            LoomType.Number => NumberValue.ToString(CultureInfo.CurrentCulture),
            LoomType.Boolean => BooleanValue.ToString(CultureInfo.CurrentCulture),
            _ => throw new UnreachableException()
        };
    }

    public bool Equals(LoomValue other)
    {
        if (Type != other.Type)
            return false;

        return Type switch
        {
            LoomType.String => StringValue == other.StringValue,
            LoomType.Number => NumberValue == other.NumberValue,
            LoomType.Boolean => BooleanValue == other.BooleanValue,
            _ => throw new UnreachableException()
        };
    }

    public override int GetHashCode()
    {
        return Type switch
        {
            LoomType.String => HashCode.Combine((int)Type, StringValue.GetHashCode()),
            LoomType.Number => HashCode.Combine((int)Type, NumberValue.GetHashCode()),
            LoomType.Boolean => HashCode.Combine((int)Type, BooleanValue.GetHashCode()),
            _ => throw new UnreachableException()
        };
    }

    public LoomValue(string stringValue)
    {
        Type = LoomType.String;
        StringValue = stringValue;
    }

    public LoomValue(decimal numberValue)
    {
        Type = LoomType.Number;
        NumberValue = numberValue;
    }

    public LoomValue(bool booleanValue)
    {
        Type = LoomType.Boolean;
        BooleanValue = booleanValue;
    }
}