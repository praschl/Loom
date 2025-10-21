using System.Text;

namespace Loom.Engine;

public record Option(string Text);

public record OptionsList(params List<Option> Options) : ContentNode
{
    public override void PushContent(IDialogEvents sharedEvents)
    {
        sharedEvents.OnOptionsReceived(this);
    }
}


public record OptionTemplate : INode
{
    public IReadOnlyCollection<IFragment>? Fragments { get; set; }
    public string? LiteralText { get; set; }
    
    public Option Evaluate()
    {
        if (!string.IsNullOrEmpty(LiteralText))
            return new Option(LiteralText);

        if (Fragments is null or { Count: 0 })
            throw new InvalidOperationException($"Fragments or LiteralText must be set.");
        
        var builder = new StringBuilder();
        
        foreach (var fragment in Fragments)
        {
            builder.Append(fragment.GetText());
        }
        
        return new Option(builder.ToString());
    }

    public OptionTemplate(IReadOnlyCollection<IFragment> fragments)
    {
        Fragments = fragments;
    }

    public OptionTemplate(string text) 
    {
        LiteralText = text;
    }
}

public record OptionsListTemplate(params List<OptionTemplate> Options) : ITemplate
{
    public INode Evaluate()
    {
        var options = Options.Select(o => o.Evaluate());
        return new OptionsList(options.ToList());
    }
}