namespace Loom.Parser.Models;

public interface ILineNode
{
    
}

public class JsIfNode : ILineNode
{
    public required int Indent { get; set; }
    public required string Condition { get; init; }
}

public class DialogLineNode : ILineNode
{
    public interface ILineNodeFragment;

    public int Indent { get; set; }
    public List<ILineNodeFragment>? Speaker { get; set; }
    
    public List<ILineNodeFragment>? Fragments { get; set; }

    public override string ToString()
    {
        return string.Concat(Speaker ?? []) + ": " + string.Concat(Fragments ?? []);
    }

    //
    
    public class TextFragment : ILineNodeFragment
    {
        public required string Text { get; init; }

        public override string ToString()
        {
            return Text;
        }
    }

    public class ScriptFragment : ILineNodeFragment
    {
        public bool HasOutput { get; init; }
        
        public required string Script { get; init; }

        public override string ToString()
        {
            return Script;
        }
    }
}