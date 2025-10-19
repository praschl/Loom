namespace Loom.Engine;

public interface IFragment
{
    string GetText();
}

public readonly record struct TextFragment(string Text) : IFragment
{
    public string GetText() => Text;
}

public readonly record struct ExpressionFragment(Func<string> evaluator) : IFragment
{
    public string GetText() => evaluator();
}
