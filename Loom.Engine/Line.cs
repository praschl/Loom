using System.Text;

namespace Loom.Engine;

public record Line(string Text) : ContentSegment
{
    public override void PushContent(IDialogEvents sharedEvents)
    {
        sharedEvents.OnLineReceived(this);
    }
}

public record LineTemplate : ITemplate
{
    public IReadOnlyCollection<IFragment>? Fragments { get; set; }
    public string? LiteralText { get; set; }
    
    public ISegment Evaluate()
    {
        if (!string.IsNullOrEmpty(LiteralText))
            return new Line(LiteralText);

        if (Fragments is null or { Count: 0 })
            throw new InvalidOperationException($"Fragments or LiteralText must be set.");
        
        var builder = new StringBuilder();
        
        foreach (var fragment in Fragments)
        {
            builder.Append(fragment.GetText());
        }
        
        return new Line(builder.ToString());
    }

    public LineTemplate(IReadOnlyCollection<IFragment> fragments)
    {
        Fragments = fragments;
    }

    public LineTemplate(string text) 
    {
        LiteralText = text;
    }
}
