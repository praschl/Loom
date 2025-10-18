namespace Loom.Engine.Tests;

public static class TestData
{
    public static class DialogRun
    {
        public static Dialog Empty() => new() { RootNode = new BlockNode() };

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

        public static Dialog With3NestedBlockNodes()
        {
            return new Dialog
            {
                RootNode = new BlockNode
                {
                    Children =
                    {
                        new Line("1"),
                        new BlockNode
                        {
                            Children =
                            {
                                new Line("1.1"),
                                new BlockNode
                                {
                                    Children =
                                    {
                                        new Line("1.1.1"),
                                        new BlockNode
                                        {
                            
                                        },
                                        new Line("1.1.2"),
                                    }
                                },
                                new Line("1.2"),
                            }
                        },
                        new Line("2"),
                    }
                }
            };
        }
    }
}