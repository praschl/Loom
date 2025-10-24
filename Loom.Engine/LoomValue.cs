using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Loom.Engine;

public readonly record struct LoomValue
{
    public enum LoomType
    {
        String,
        Number, // (use decimal)
        Boolean,
    }
    
    public LoomType Type { get; }

    public string? StringValue { get; }
    public decimal? NumberValue { get; }
    public bool? BooleanValue { get; }

    public override string ToString()
    {
        return Type switch
        {
            LoomType.String => StringValue!,
            LoomType.Number => NumberValue!.Value.ToString(CultureInfo.CurrentCulture),
            LoomType.Boolean => BooleanValue!.Value.ToString(CultureInfo.CurrentCulture),
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
            LoomType.String => HashCode.Combine((int)Type, StringValue!.GetHashCode()),
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