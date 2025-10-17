using FluentAssertions;

namespace Loom.Engine.Tests;

public class DialogDefinitionTests
{
    [Fact]
    public void Start_returns_a_dialog()
    {
        var dialog = TestData.Dialog.Empty();

        dialog.Should().NotBeNull();
    }
}

public class DialogTests
{
    private Dialog _dialog;

    private int _nodesCount;
    private Node? _lastNode;

    private int _linesCount;
    private Line? _lastLine;

    private int _optionsCount;
    private OptionsList? _lastOptionsList;
    private bool _dialogFinished;

    private void SetupEvents(Dialog dialog)
    {
        // NOTE: StartDialog() will not send a node!
        // this is still the low level implementation without UI or Components

        dialog.DialogEvents.OnLine += line =>
        {
            _nodesCount++;
            _linesCount++;
            _lastLine = line;
            _lastNode = line;
        };

        dialog.DialogEvents.OnOptionsList += options =>
        {
            _nodesCount++;
            _optionsCount++;
            _lastOptionsList = options;
            _lastNode = options;
        };

        dialog.DialogEvents.DialogFinished += () =>
        {
            _dialogFinished = true;
        };
        
        _dialog = dialog;
    }

    [Fact]
    public void Advance_sends_a_line()
    {
        SetupEvents(TestData.Dialog.With3Lines().StartDialog());

        _dialog.Advance();

        _nodesCount.Should().Be(1);
        _linesCount.Should().Be(1);
        _lastLine.Text.Should().Be("One");
    }
    
    [Fact]
    public void Advance_sends_three_lines_in_order()
    {
        SetupEvents(TestData.Dialog.With3Lines().StartDialog());

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
    public void Advance_raises_DialogFinished_when_already_on_last_node()
    {
        SetupEvents(TestData.Dialog.With3Lines().StartDialog());

        _dialog.Advance();
        _dialogFinished.Should().BeFalse();
        
        _dialog.Advance();
        _dialogFinished.Should().BeFalse();
                
        _dialog.Advance();
        _dialogFinished.Should().BeFalse();
        
        _dialog.Advance();
        _dialogFinished.Should().BeTrue();
    }

    [Fact]
    public void Advance_sends_options_after_first_line()
    {
        SetupEvents(TestData.Dialog.With1OptionsList().StartDialog());

        _dialog.Advance();
        _dialog.Advance();
        
        _linesCount.Should().Be(1);
        _optionsCount.Should().Be(1);

        _lastNode.Should().BeOfType<OptionsList>();
        _lastOptionsList.Options.Should().HaveCount(3);
    }
}