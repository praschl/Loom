namespace Loom.Parser.Models;

public class LineNode
{
    public interface ILineNodeFragment;

    public class TextFragment : ILineNodeFragment
    {
        public required string Text { get; init; }
    }

    public string? Speaker { get; set; }
    
    public List<ILineNodeFragment>? Fragments { get; set; }
}