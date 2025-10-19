using System.Text;

namespace Loom.Engine;

public record Line(string Text) : ContentNode
{
    public override void PushContent(IDialogEvents sharedEvents)
    {
        sharedEvents.OnLineReceived(this);
    }
}

public record LineTemplate : Node, IEvaluateable
{
    public IReadOnlyCollection<IFragment>? Fragments { get; set; }
    public string? LiteralText { get; set; }
    
    public Node Evaluate()
    {
        if (!string.IsNullOrEmpty(LiteralText))
            return new Line(LiteralText);

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
