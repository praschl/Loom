namespace Loom.Engine;

public class Dialog
{
    public BlockNode RootNode { get; set; }
    
    public DialogRun StartDialog()
    {
        return new DialogRun(RootNode);
    }
}
