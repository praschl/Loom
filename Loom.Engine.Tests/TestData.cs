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
                        new LineTemplate("One"),
                        new LineTemplate("Two"),
                        new LineTemplate("Three"),
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
                        new LineTemplate("One"),
                        new OptionsList(new Option("Option 1"), new Option("Option 2"), new Option("Option 3")),
                        new LineTemplate("Three"),
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
                        new LineTemplate("1"),
                        new BlockNode("B in 1")
                        {
                            Children =
                            {
                                new LineTemplate("1.1"),
                                new BlockNode("B in 1.1")
                                {
                                    Children =
                                    {
                                        new LineTemplate("1.1.1"),
                                        new BlockNode("B in 1.1.1")
                                        {
                                        },
                                        new LineTemplate("1.1.2"),
                                    }
                                },
                                new LineTemplate("1.2"),
                            }
                        },
                        new LineTemplate("2"),
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
                        new LineTemplate("Start"),
                        new ConditionalNode()
                        {
                            Condition = () => condition,
                            WhenTrue = new BlockNode("true")
                            {
                                Children = { new LineTemplate("True line") }
                            },
                            WhenFalse = new BlockNode("false")
                            {
                                Children = { new LineTemplate("False line") }
                            }
                        },
                        new LineTemplate("End")
                    }
                }
            };
        }

        public static Dialog WithAction(Action action)
        {
            return new Dialog
            {
                RootNode = new BlockNode("root")
                {
                    Children =
                    {
                        new LineTemplate("Start"),
                        new ActionNode(action) { Name = "TestAction" },
                        new LineTemplate("End")
                    }
                }
            };
        }

        public static Dialog WithLineTemplate(bool condition)
        {
            return new Dialog
            {
                RootNode = new BlockNode("root")
                {
                    Children =
                    {
                        new LineTemplate([
                            new TextFragment("This is "),
                            new ExpressionFragment(() => condition ? "only" : "just"),
                            new TextFragment(" the beginning")
                        ])
                    }
                }
            };
        }
    }
}