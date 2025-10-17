namespace Loom.Engine.Tests;

public static class TestData
{
    public static class Dialog
    {
        public static DialogDefinition Empty() => new DialogDefinition();
        
        public static DialogDefinition With3Lines()
        {
            return new DialogDefinition
            {
                Lines =
                {
                    new Line("One"),
                    new Line("Two"),
                    new Line("Three"),
                }
            };
        }
    }
}