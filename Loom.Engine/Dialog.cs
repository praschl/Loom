namespace Loom.Engine;

public class Dialog
{
    public BlockNode RootNode { get; set; }
    
    public DialogSession StartDialog()
    {
        return new DialogSession(RootNode);
    }
}
