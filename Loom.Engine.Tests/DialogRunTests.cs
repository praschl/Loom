using FluentAssertions;

namespace Loom.Engine.Tests;

public class DialogRunTests
{
    private DialogRun _dialogRun = null!;

    private int _nodesCount;
    private Node? _lastNode;

    private int _linesCount;
    private Line? _lastLine;

    private int _optionsCount;
    private OptionsList? _lastOptionsList;
    private bool _dialogFinished;

    private void SetupEvents(DialogRun dialogRun)
    {
        // NOTE: StartDialog() will not send a node!
        // this is still the low level implementation without UI or Components

        dialogRun.DialogEvents.LineReceived += line =>
        {
            _nodesCount++;
            _linesCount++;
            _lastLine = line;
            _lastNode = line;
        };

        dialogRun.DialogEvents.OptionsReceived += options =>
        {
            _nodesCount++;
            _optionsCount++;
            _lastOptionsList = options;
            _lastNode = options;
        };

        dialogRun.DialogEvents.DialogFinished += () =>
        {
            _dialogFinished = true;
        };
        
        _dialogRun = dialogRun;
    }

    [Fact]
    public void Advance_sends_a_line()
    {
        SetupEvents(TestData.DialogRun.With3Lines().StartDialog());

        _dialogRun.Advance();

        _nodesCount.Should().Be(1);
        _linesCount.Should().Be(1);
        _lastLine.Text.Should().Be("One");
    }
    
    [Fact]
    public void Advance_sends_three_lines_in_order()
    {
        SetupEvents(TestData.DialogRun.With3Lines().StartDialog());

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
    public void Advance_raises_DialogFinished_when_already_on_last_node()
    {
        SetupEvents(TestData.DialogRun.With3Lines().StartDialog());

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
    public void Advance_raises_DialogFinished_when_already_on_last_node_with_options()
    {
        SetupEvents(TestData.DialogRun.With1OptionsList().StartDialog());

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
        SetupEvents(TestData.DialogRun.With1OptionsList().StartDialog());

        _dialogRun.Advance();
        _dialogRun.Advance();
        
        _linesCount.Should().Be(1);
        _optionsCount.Should().Be(1);

        _lastNode.Should().BeOfType<OptionsList>();
        _lastOptionsList.Options.Should().HaveCount(3);
    }
}