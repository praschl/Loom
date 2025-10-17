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
    private bool _dialogStarted;

    private void Setup(DialogRun dialogRun)
    {
        // NOTE: StartDialog() will not send a node!
        // this is still the low level implementation without UI or Components

        dialogRun.DialogEvents.DialogStarted += () =>
        {
            _dialogStarted = true;
        };
        
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
}