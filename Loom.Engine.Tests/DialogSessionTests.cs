using FluentAssertions;
using Xunit.Abstractions;

namespace Loom.Engine.Tests;

public class DialogSessionTests(ITestOutputHelper console)
{
    private DialogSession _dialog = null!;

    private int _nodesCount;
    private INode? _lastNode;

    private int _linesCount;
    private Line _lastLine = null!;

    private int _optionsCount;
    private OptionsList _lastOptionsList = null!;
    private bool _dialogFinished;
    private bool _dialogStarted;

    private void Setup(DialogSession dialogSession)
    {
        // NOTE: StartDialog() will not send a node!
        // this is still the low level implementation without UI or Components

        int indent = 0;
        
        dialogSession.DialogEvents.DialogStarted += () =>
        {
            if (_dialogStarted)
                Assert.Fail("Dialog has already been started");
            _dialogStarted = true;
        };

        dialogSession.DialogEvents.BlockStarted += bl =>
        {
            console.WriteLine($"{new string(' ', indent)}Block started: {bl.Name}");
            indent += 2;
        };
        
        dialogSession.DialogEvents.LineReceived += line =>
        {
            console.WriteLine($"{new string(' ', indent)}Line received: {line}");
            _nodesCount++;
            _linesCount++;
            _lastLine = line;
            _lastNode = line;
        };

        dialogSession.DialogEvents.OptionsReceived += options =>
        {
            console.WriteLine($"{new string(' ', indent)}Options received: {options}");
            _nodesCount++;
            _optionsCount++;
            _lastOptionsList = options;
            _lastNode = options;
        };
        
        dialogSession.DialogEvents.BlockFinishing += bl =>
        {
            indent -= 2;
            console.WriteLine($"{new string(' ', indent)}Block finishing: {bl.Name}");
        };
        
        dialogSession.DialogEvents.DialogFinished += () =>
        {
            if (_dialogFinished)
                Assert.Fail("Dialog has already been finished");
            _dialogFinished = true;
        };

        dialogSession.DialogEvents.Log += (text, node) =>
        {
            console.WriteLine($"{new string(' ', indent)} ({node?.GetType().Name}) {text}");
        };
        
        _dialog = dialogSession;
    }

    [Fact]
    public void Advance_sends_a_line()
    {
        Setup(TestData.DialogSession.With_3_Lines().StartDialog());

        _dialog.Advance();

        _nodesCount.Should().Be(1);
        _linesCount.Should().Be(1);
        _lastLine.Text.Should().Be("One");
    }

    [Fact]
    public void Advance_sends_three_lines_in_order()
    {
        Setup(TestData.DialogSession.With_3_Lines().StartDialog());

        // one
        _dialog.Advance();

        _nodesCount.Should().Be(1);
        _linesCount.Should().Be(1);
        _lastLine.Text.Should().Be("One");

        // two
        _dialog.Advance();

        _nodesCount.Should().Be(2);
        _linesCount.Should().Be(2);
        _lastLine.Text.Should().Be("Two");

        // three
        _dialog.Advance();

        _nodesCount.Should().Be(3);
        _linesCount.Should().Be(3);
        _lastLine.Text.Should().Be("Three");
    }

    [Fact]
    public void Advance_raises_DialogStarted_when_advancing_to_first_line()
    {
        Setup(TestData.DialogSession.With_3_Lines().StartDialog());

        _dialog.Advance();

        _dialogStarted.Should().BeTrue();
        _dialogStarted = false;

        _dialog.Advance();

        _dialogStarted.Should().BeFalse();
    }

    [Fact]
    public void Advance_raises_DialogFinished_when_already_on_last_node()
    {
        Setup(TestData.DialogSession.With_3_Lines().StartDialog());

        _dialog.Advance();
        _dialogFinished.Should().BeFalse();

        _dialog.Advance();
        _dialogFinished.Should().BeFalse();

        _dialog.Advance();
        _dialogFinished.Should().BeFalse();

        // after third line, that line should still be displayed 

        _dialog.Advance();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_raises_DialogStarted_and_DialogFinished_when_root_is_empty()
    {
        Setup(TestData.DialogSession.Empty().StartDialog());

        _dialogStarted.Should().BeFalse();
        _dialogFinished.Should().BeFalse();

        _dialog.Advance();

        _dialogStarted.Should().BeTrue();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_raises_DialogFinished_when_already_on_last_node_with_options()
    {
        Setup(TestData.DialogSession.With_OptionsList().StartDialog());

        _dialog.Advance();
        _dialogFinished.Should().BeFalse();

        _dialog.Advance();
        _dialogFinished.Should().BeFalse();

        _dialog.SelectOption(0);
        _dialogFinished.Should().BeFalse();

        // after third line, that line should still be displayed 

        _dialog.Advance();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_sends_options_after_first_line()
    {
        Setup(TestData.DialogSession.With_OptionsList().StartDialog());

        _dialog.Advance();
        _dialog.Advance();

        _linesCount.Should().Be(1);
        _optionsCount.Should().Be(1);

        _lastNode.Should().BeOfType<OptionsList>();
        _lastOptionsList.Options.Should().HaveCount(3);
    }

    [Fact]
    public void Advance_throws_when_current_node_is_not_a_Line()
    {
        Setup(TestData.DialogSession.With_OptionsList().StartDialog());

        _dialog.Advance();
        _dialog.Advance();

        Action advance = () => _dialog.Advance();
        advance.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SelectOption_throws_when_current_node_is_not_an_OptionList()
    {
        Setup(TestData.DialogSession.With_OptionsList().StartDialog());

        Action selectOption = () => _dialog.SelectOption(0);
        selectOption.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Advance_visits_nested_BlockNodes()
    {
        Setup(TestData.DialogSession.With_3_nested_BlockNodes().StartDialog());

        _dialog.Advance();
        _lastLine.Text.Should().Be("1");

        _dialog.Advance();
        _lastLine.Text.Should().Be("1.1");

        _dialog.Advance();
        _lastLine.Text.Should().Be("1.1.1");

        _dialog.Advance();
        _lastLine.Text.Should().Be("1.1.2");

        _dialog.Advance();
        _lastLine.Text.Should().Be("1.2");

        _dialog.Advance();
        _lastLine.Text.Should().Be("2");
        _dialogFinished.Should().BeFalse();

        _dialog.Advance();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_throws_when_finished()
    {
        Setup(TestData.DialogSession.With_3_Lines().StartDialog());

        _dialog.Advance();
        _dialog.Advance();
        _dialog.Advance();
        _dialog.Advance();

        _dialogFinished.Should().BeTrue();

        Action advance = () => _dialog.Advance();
        advance.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Advance_goes_into_correct_conditional_block(bool condition)
    {
        Setup(TestData.DialogSession.With_ConditionalBlock(condition).StartDialog());
        
        _dialog.Advance();
        _lastLine.Text.Should().Be("Start");
        
        _dialog.Advance();
        _lastLine.Text.Should().Be($"{condition} line");
        
        _dialog.Advance();
        _lastLine.Text.Should().Be("End");
        
        _dialog.Advance();
        _dialogFinished.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Advance_goes_into_correct_conditional_block_with_no_false_block(bool condition)
    {
        Setup(TestData.DialogSession.With_ConditionalBlock_where_FalseBlock_is_empty(condition).StartDialog());
        
        _dialog.Advance();
        _lastLine.Text.Should().Be("Start");

        if (condition)
        {
            _dialog.Advance();
            _lastLine.Text.Should().Be($"{condition} line");
        }
        
        _dialog.Advance();
        _lastLine.Text.Should().Be("End");
        
        _dialog.Advance();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_executes_action_and_returns_next_line()
    {
        bool called = false;
        Setup(TestData.DialogSession.With_Action(() => called = true).StartDialog());
        
        _dialog.Advance();
        called.Should().BeFalse();
        
        _dialog.Advance();
        called.Should().BeTrue();
        _lastLine.Text.Should().Be("End");
        
        _dialog.Advance();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_executes_all_actions_immediately_even_if_they_were_last_and_immediately_ends_dialog()
    {
        bool called1 = false;
        bool called2 = false;
        
        Setup(TestData.DialogSession.With_2_Actions_as_finish(() => called1 = true, () => called2 = true).StartDialog());
        
        _dialog.Advance();
        called1.Should().BeFalse();
        called2.Should().BeFalse();
        
        _dialog.Advance();
        called1.Should().BeTrue();
        called2.Should().BeTrue();
        _dialogFinished.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Advance_returns_evaluated_line(bool condition)
    {
        Setup(TestData.DialogSession.With_Line_with_Fragments(condition).StartDialog());

        _dialog.Advance();

        string onlyJust = condition ? "only" : "just";
        
        _lastLine.Text.Should().Be($"This is {onlyJust} the beginning");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Advance_returns_evaluated_options(bool condition)
    {
        Setup(TestData.DialogSession.With_OptionsList_with_Fragments(condition).StartDialog());

        _dialog.Advance();
        _dialog.Advance();
        
        string goodbad = condition ? "is a good" : "is a bad";

        _lastOptionsList.Options[0].Text.Should().Be($"This {goodbad} Option");
    }
}