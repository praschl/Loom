namespace Loom.Engine.Tests;

public static class TestData
{
    public static class DialogSession
    {
        public static Dialog Empty() => new() { RootNode = new BlockNode("Empty") };

        public static Dialog With3Lines()
        {
            return new Dialog
            {
                RootNode = new BlockNode("With3Lines")
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
                RootNode = new BlockNode("With1OptionsList")
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
                RootNode = new BlockNode("root")
                {
                    Children =
                    {
                        new Line("1"),
                        new BlockNode("B in 1")
                        {
                            Children =
                            {
                                new Line("1.1"),
                                new BlockNode("B in 1.1")
                                {
                                    Children =
                                    {
                                        new Line("1.1.1"),
                                        new BlockNode("B in 1.1.1")
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

        public static Dialog With1ConditionalBlock(bool condition)
        {
            return new Dialog
            {
                RootNode = new BlockNode("root")
                {
                    Children =
                    {
                        new Line("Start"),
                        new ConditionalNode()
                        {
                            Condition = () => condition,
                            WhenTrue = new BlockNode("true")
                            {
                                Children = { new Line("True line") }
                            },
                            WhenFalse = new BlockNode("false")
                            {
                                Children = { new Line("False line") }
                            }
                        },
                        new Line("End")
                    }
                }
            };
        }
    }
}