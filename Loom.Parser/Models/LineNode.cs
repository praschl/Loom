namespace Loom.Parser.Models;

public class LineNode
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
        public required string Script { get; init; }

        public override string ToString()
        {
            return Script;
        }
    }
}