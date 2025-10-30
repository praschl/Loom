namespace Loom.Engine;

public class Dialog
{
    public required BlockSegment RootSegment { get; set; }
    
    public DialogSession StartDialog()
    {
        return new DialogSession(RootSegment);
    }
}
