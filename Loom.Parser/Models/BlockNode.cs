namespace Loom.Parser.Models;

public class BlockNode
{
    public string? Title { get; set; }
    public List<string>? Tags { get; set; }
    public List<LineNode>? Lines { get; set; }

    public override string ToString()
    {
        return $"title: {Title} - Tags: {string.Join(',', Tags?? [])} - Lines: {Lines?.Count ?? 0})";
    }
}