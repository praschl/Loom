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
    private readonly Dialog _dialog;

    private int _linesSent;
    private Line? _lastLine;

    public DialogTests()
    {
        _dialog = TestData.Dialog.With3Lines().StartDialog();

        // NOTE: StartDialog() will not send a line!
        // this is still the low level implementation without UI or Components

        _dialog.DialogEvents.OnLine += line =>
        {
            _linesSent++;
            _lastLine = line;
        };
    }

    [Fact]
    public void Advance_sends_a_line()
    {
        _dialog.Advance();

        _linesSent.Should().Be(1);
        _lastLine.Text.Should().Be("One");
    }
    
    [Fact]
    public void Advance_sends_three_lines_in_order()
    {
        // one
        _dialog.Advance();

        _linesSent.Should().Be(1);
        _lastLine.Text.Should().Be("One");

        // two
        _dialog.Advance();

        _linesSent.Should().Be(2);
        _lastLine.Text.Should().Be("Two");
        
        // three
        _dialog.Advance();

        _linesSent.Should().Be(3);
        _lastLine.Text.Should().Be("Three");
    }
}