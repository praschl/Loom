using FluentAssertions;
using Xunit.Abstractions;

namespace Loom.Engine.Tests;

public class DialogRunTests(ITestOutputHelper console)
{
    private DialogRun _dialogRun = null!;

    private int _nodesCount;
    private Node? _lastNode;

    private int _linesCount;
    private Line? _lastLine;

    private int _optionsCount;
    private OptionsList? _lastOptionsList;
    private bool _dialogFinished;
    private bool _dialogStarted;

    private void Setup(DialogRun dialogRun)
    {
        // NOTE: StartDialog() will not send a node!
        // this is still the low level implementation without UI or Components

        int indent = 0;
        
        dialogRun.DialogEvents.DialogStarted += () =>
        {
            if (_dialogStarted)
                Assert.Fail("Dialog has already been started");
            _dialogStarted = true;
        };

        dialogRun.DialogEvents.BlockStarted += bl =>
        {
            console.WriteLine($"{new string(' ', indent)}Block started: {bl.Name}");
            indent += 2;
        };
        
        dialogRun.DialogEvents.LineReceived += line =>
        {
            console.WriteLine($"{new string(' ', indent)}Line received: {line}");
            _nodesCount++;
            _linesCount++;
            _lastLine = line;
            _lastNode = line;
        };

        dialogRun.DialogEvents.OptionsReceived += options =>
        {
            console.WriteLine($"{new string(' ', indent)}Options received: {options}");
            _nodesCount++;
            _optionsCount++;
            _lastOptionsList = options;
            _lastNode = options;
        };
        
        dialogRun.DialogEvents.BlockFinishing += bl =>
        {
            indent -= 2;
            console.WriteLine($"{new string(' ', indent)}Block finishing: {bl.Name}");
        };
        
        dialogRun.DialogEvents.DialogFinished += () =>
        {
            if (_dialogFinished)
                Assert.Fail("Dialog has already been finished");
            _dialogFinished = true;
        };

        _dialogRun = dialogRun;
    }

    [Fact]
    public void Advance_sends_a_line()
    {
        Setup(TestData.DialogRun.With3Lines().StartDialog());

        _dialogRun.Advance();

        _nodesCount.Should().Be(1);
        _linesCount.Should().Be(1);
        _lastLine.Text.Should().Be("One");
    }

    [Fact]
    public void Advance_sends_three_lines_in_order()
    {
        Setup(TestData.DialogRun.With3Lines().StartDialog());

        // one
        _dialogRun.Advance();

        _nodesCount.Should().Be(1);
        _linesCount.Should().Be(1);
        _lastLine.Text.Should().Be("One");

        // two
        _dialogRun.Advance();

        _nodesCount.Should().Be(2);
        _linesCount.Should().Be(2);
        _lastLine.Text.Should().Be("Two");

        // three
        _dialogRun.Advance();

        _nodesCount.Should().Be(3);
        _linesCount.Should().Be(3);
        _lastLine.Text.Should().Be("Three");
    }

    [Fact]
    public void Advance_raises_DialogStarted_when_advancing_to_first_line()
    {
        Setup(TestData.DialogRun.With3Lines().StartDialog());

        _dialogRun.Advance();

        _dialogStarted.Should().BeTrue();
        _dialogStarted = false;

        _dialogRun.Advance();

        _dialogStarted.Should().BeFalse();
    }

    [Fact]
    public void Advance_raises_DialogFinished_when_already_on_last_node()
    {
        Setup(TestData.DialogRun.With3Lines().StartDialog());

        _dialogRun.Advance();
        _dialogFinished.Should().BeFalse();

        _dialogRun.Advance();
        _dialogFinished.Should().BeFalse();

        _dialogRun.Advance();
        _dialogFinished.Should().BeFalse();

        // after third line, that line should still be displayed 

        _dialogRun.Advance();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_raises_DialogStarted_and_DialogFinished_when_root_is_empty()
    {
        Setup(TestData.DialogRun.Empty().StartDialog());

        _dialogStarted.Should().BeFalse();
        _dialogFinished.Should().BeFalse();

        _dialogRun.Advance();

        _dialogStarted.Should().BeTrue();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_raises_DialogFinished_when_already_on_last_node_with_options()
    {
        Setup(TestData.DialogRun.With1OptionsList().StartDialog());

        _dialogRun.Advance();
        _dialogFinished.Should().BeFalse();

        _dialogRun.Advance();
        _dialogFinished.Should().BeFalse();

        _dialogRun.SelectOption(0);
        _dialogFinished.Should().BeFalse();

        // after third line, that line should still be displayed 

        _dialogRun.Advance();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_sends_options_after_first_line()
    {
        Setup(TestData.DialogRun.With1OptionsList().StartDialog());

        _dialogRun.Advance();
        _dialogRun.Advance();

        _linesCount.Should().Be(1);
        _optionsCount.Should().Be(1);

        _lastNode.Should().BeOfType<OptionsList>();
        _lastOptionsList.Options.Should().HaveCount(3);
    }

    [Fact]
    public void Advance_throws_when_current_node_is_not_a_Line()
    {
        Setup(TestData.DialogRun.With1OptionsList().StartDialog());

        _dialogRun.Advance();
        _dialogRun.Advance();

        Action advance = () => _dialogRun.Advance();
        advance.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SelectOption_throws_when_current_node_is_not_an_OptionList()
    {
        Setup(TestData.DialogRun.With1OptionsList().StartDialog());

        Action selectOption = () => _dialogRun.SelectOption(0);
        selectOption.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Advance_visits_nested_BlockNodes()
    {
        Setup(TestData.DialogRun.With3NestedBlockNodes().StartDialog());

        _dialogRun.Advance();
        _lastLine.Text.Should().Be("1");

        _dialogRun.Advance();
        _lastLine.Text.Should().Be("1.1");

        _dialogRun.Advance();
        _lastLine.Text.Should().Be("1.1.1");

        _dialogRun.Advance();
        _lastLine.Text.Should().Be("1.1.2");

        _dialogRun.Advance();
        _lastLine.Text.Should().Be("1.2");

        _dialogRun.Advance();
        _lastLine.Text.Should().Be("2");
        _dialogFinished.Should().BeFalse();

        _dialogRun.Advance();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_throws_when_finished()
    {
        Setup(TestData.DialogRun.With3Lines().StartDialog());

        _dialogRun.Advance();
        _dialogRun.Advance();
        _dialogRun.Advance();
        _dialogRun.Advance();

        _dialogFinished.Should().BeTrue();

        Action advance = () => _dialogRun.Advance();
        advance.Should().Throw<InvalidOperationException>();
    }
}