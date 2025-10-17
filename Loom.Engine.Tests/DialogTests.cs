using FluentAssertions;

namespace Loom.Engine.Tests;

public class DialogTests
{
    [Fact]
    public void Start_returns_a_dialog()
    {
        var dialog = TestData.DialogRun.Empty();

        dialog.Should().NotBeNull();
    }
}