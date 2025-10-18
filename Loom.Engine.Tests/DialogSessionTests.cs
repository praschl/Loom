using FluentAssertions;
using Xunit.Abstractions;

namespace Loom.Engine.Tests;

public class DialogSessionTests(ITestOutputHelper console)
{
    private DialogSession _dialogSession = null!;

    private int _nodesCount;
    private Node? _lastNode;

    private int _linesCount;
    private Line? _lastLine;

    private int _optionsCount;
    private OptionsList? _lastOptionsList;
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

        _dialogSession = dialogSession;
    }

    [Fact]
    public void Advance_sends_a_line()
    {
        Setup(TestData.DialogSession.With3Lines().StartDialog());

        _dialogSession.Advance();

        _nodesCount.Should().Be(1);
        _linesCount.Should().Be(1);
        _lastLine.Text.Should().Be("One");
    }

    [Fact]
    public void Advance_sends_three_lines_in_order()
    {
        Setup(TestData.DialogSession.With3Lines().StartDialog());

        // one
        _dialogSession.Advance();

        _nodesCount.Should().Be(1);
        _linesCount.Should().Be(1);
        _lastLine.Text.Should().Be("One");

        // two
        _dialogSession.Advance();

        _nodesCount.Should().Be(2);
        _linesCount.Should().Be(2);
        _lastLine.Text.Should().Be("Two");

        // three
        _dialogSession.Advance();

        _nodesCount.Should().Be(3);
        _linesCount.Should().Be(3);
        _lastLine.Text.Should().Be("Three");
    }

    [Fact]
    public void Advance_raises_DialogStarted_when_advancing_to_first_line()
    {
        Setup(TestData.DialogSession.With3Lines().StartDialog());

        _dialogSession.Advance();

        _dialogStarted.Should().BeTrue();
        _dialogStarted = false;

        _dialogSession.Advance();

        _dialogStarted.Should().BeFalse();
    }

    [Fact]
    public void Advance_raises_DialogFinished_when_already_on_last_node()
    {
        Setup(TestData.DialogSession.With3Lines().StartDialog());

        _dialogSession.Advance();
        _dialogFinished.Should().BeFalse();

        _dialogSession.Advance();
        _dialogFinished.Should().BeFalse();

        _dialogSession.Advance();
        _dialogFinished.Should().BeFalse();

        // after third line, that line should still be displayed 

        _dialogSession.Advance();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_raises_DialogStarted_and_DialogFinished_when_root_is_empty()
    {
        Setup(TestData.DialogSession.Empty().StartDialog());

        _dialogStarted.Should().BeFalse();
        _dialogFinished.Should().BeFalse();

        _dialogSession.Advance();

        _dialogStarted.Should().BeTrue();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_raises_DialogFinished_when_already_on_last_node_with_options()
    {
        Setup(TestData.DialogSession.With1OptionsList().StartDialog());

        _dialogSession.Advance();
        _dialogFinished.Should().BeFalse();

        _dialogSession.Advance();
        _dialogFinished.Should().BeFalse();

        _dialogSession.SelectOption(0);
        _dialogFinished.Should().BeFalse();

        // after third line, that line should still be displayed 

        _dialogSession.Advance();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_sends_options_after_first_line()
    {
        Setup(TestData.DialogSession.With1OptionsList().StartDialog());

        _dialogSession.Advance();
        _dialogSession.Advance();

        _linesCount.Should().Be(1);
        _optionsCount.Should().Be(1);

        _lastNode.Should().BeOfType<OptionsList>();
        _lastOptionsList.Options.Should().HaveCount(3);
    }

    [Fact]
    public void Advance_throws_when_current_node_is_not_a_Line()
    {
        Setup(TestData.DialogSession.With1OptionsList().StartDialog());

        _dialogSession.Advance();
        _dialogSession.Advance();

        Action advance = () => _dialogSession.Advance();
        advance.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SelectOption_throws_when_current_node_is_not_an_OptionList()
    {
        Setup(TestData.DialogSession.With1OptionsList().StartDialog());

        Action selectOption = () => _dialogSession.SelectOption(0);
        selectOption.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Advance_visits_nested_BlockNodes()
    {
        Setup(TestData.DialogSession.With3NestedBlockNodes().StartDialog());

        _dialogSession.Advance();
        _lastLine.Text.Should().Be("1");

        _dialogSession.Advance();
        _lastLine.Text.Should().Be("1.1");

        _dialogSession.Advance();
        _lastLine.Text.Should().Be("1.1.1");

        _dialogSession.Advance();
        _lastLine.Text.Should().Be("1.1.2");

        _dialogSession.Advance();
        _lastLine.Text.Should().Be("1.2");

        _dialogSession.Advance();
        _lastLine.Text.Should().Be("2");
        _dialogFinished.Should().BeFalse();

        _dialogSession.Advance();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_throws_when_finished()
    {
        Setup(TestData.DialogSession.With3Lines().StartDialog());

        _dialogSession.Advance();
        _dialogSession.Advance();
        _dialogSession.Advance();
        _dialogSession.Advance();

        _dialogFinished.Should().BeTrue();

        Action advance = () => _dialogSession.Advance();
        advance.Should().Throw<InvalidOperationException>();
    }
}