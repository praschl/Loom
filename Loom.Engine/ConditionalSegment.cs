namespace Loom.Engine;

public record ConditionalSegment : ISegment, ITemplate
{
    public required Func<bool> Condition { get; set; }

    public required BlockSegment WhenTrue { get; set; }
    public BlockSegment? WhenFalse { get; set; }

    public BlockSegment? GetCorrectSegment()
    {
        if (Condition())
            return WhenTrue;

        return WhenFalse;
    }

    public ISegment Evaluate()
    {
        return this;
    }
}