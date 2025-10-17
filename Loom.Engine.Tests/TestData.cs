namespace Loom.Engine.Tests;

public static class TestData
{
    public static class Dialog
    {
        public static DialogDefinition Empty() => new ();
        
        public static DialogDefinition With3Lines()
        {
            return new DialogDefinition
            {
                Nodes =
                {
                    new Line("One"),
                    new Line("Two"),
                    new Line("Three"),
                }
            };
        }

        public static DialogDefinition With1OptionsList()
        {
            return new DialogDefinition
            {
                Nodes =
                {
                    new Line("One"),
                    new OptionsList(new Option("Option 1"), new Option("Option 2"), new Option("Option 3")),
                    new Line("Three"),
                }
            };
        }
    }
}