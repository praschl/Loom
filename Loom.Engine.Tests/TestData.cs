namespace Loom.Engine.Tests;

public static class TestData
{
    public static class DialogSession
    {
        public static Dialog Empty() => new() { RootNode = new BlockNode("Empty") };

        public static Dialog With_3_Lines()
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

        public static Dialog With_OptionsList()
        {
            return new Dialog
            {
                RootNode = new BlockNode("With1OptionsList")
                {
                    Children =
                    {
                        new LineTemplate("One"),
                        new OptionsListTemplate(new OptionTemplate("Option 1"), new OptionTemplate("Option 2"), new OptionTemplate("Option 3")),
                        new LineTemplate("Three"),
                    }
                }
            };
        }

        public static Dialog With_3_nested_BlockNodes()
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

        public static Dialog With_ConditionalBlock(bool condition)
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
        
        public static Dialog With_ConditionalBlock_where_FalseBlock_is_empty(bool condition)
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
                            }
                        },
                        new LineTemplate("End")
                    }
                }
            };
        }

        public static Dialog With_Action(Action action)
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
        
        public static Dialog With_2_Actions_as_finish(Action action1 , Action action2)
        {
            return new Dialog
            {
                RootNode = new BlockNode("root")
                {
                    Children =
                    {
                        new LineTemplate("Start"),
                        new ActionNode(action1) { Name = "TestAction 1" },
                        new ActionNode(action2) { Name = "TestAction 2" }
                    }
                }
            };
        }

        public static Dialog With_Line_with_Fragments(bool condition)
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
        
        public static Dialog With_OptionsList_with_Fragments(bool condition)
        {
            return new Dialog
            {
                RootNode = new BlockNode("With1OptionsList")
                {
                    Children =
                    {
                        new LineTemplate("One"),
                        new OptionsListTemplate(new OptionTemplate([new TextFragment("This "), new ExpressionFragment(() => condition ? "is a good" : "is a bad"), new TextFragment(" Option")])),
                        new LineTemplate("Three"),
                    }
                }
            };
        }
    }
}