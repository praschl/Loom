namespace Loom.Engine.Tests;

public static class TestData
{
    public static class DialogRun
    {
        public static Dialog Empty() => new();

        public static Dialog With3Lines()
        {
            return new Dialog
            {
                RootNode = new BlockNode
                {
                    Children =
                    {
                        new Line("One"),
                        new Line("Two"),
                        new Line("Three"),
                    }
                }
            };
        }

        public static Dialog With1OptionsList()
        {
            return new Dialog
            {
                RootNode = new BlockNode
                {
                    Children =
                    {
                        new Line("One"),
                        new OptionsList(new Option("Option 1"), new Option("Option 2"), new Option("Option 3")),
                        new Line("Three"),
                    }
                }
            };
        }
    }
}