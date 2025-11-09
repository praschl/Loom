namespace Loom.Parser.Models;

public class FileNode
{
    public List<BlockNode>? ParsedBlocks { get; set; }

    public override string ToString()
    {
        return $"ParsedBlocks {ParsedBlocks?.Count ?? 0}";
    }
}