using Antlr4.Runtime;
using FluentAssertions;
using Loom.Parser.Generated;
using Loom.Parser.Models;
using Loom.Parser.Services;

namespace Loom.Parser.Tests;

public class LoomDialogVisitorTests
{
    private LoomParser CreateParser(string input)
    {
        var inputStream = new AntlrInputStream(input);
        var lexer = new LoomLexer(inputStream);
        var tokens = new CommonTokenStream(lexer);
        return new LoomParser(tokens);
    }

    [Fact]
    public void Parses_SimpleBlock_WithTitleAndTags()
    {
        // Arrange
        var text = """
                   title: Grossvaters Haus
                   tags: eins zwei drei
                   -----------
                   Michael: Hallo Welt!
                   Chris: Hi ebenfalls, das geht!
                   ===

                   title : Keller
                   ---
                   Noch was ohne Name...
                   Simon: Was ist das hier?
                   =====
                   """;

        var parser = CreateParser(text);
        var tree = parser.file();

        // Act
        var visitor = new LoomDialogVisitor();
        var file = visitor.VisitFile(tree);

        // Assert
        Assert.NotNull(tree);
        Assert.Equal(0, parser.NumberOfSyntaxErrors);

        file.Should().NotBeNull();
        file.ParsedBlocks.Should().NotBeNull();
        file.ParsedBlocks.Should().HaveCount(2);

        file.ParsedBlocks[0].Title.Should().Be("Grossvaters Haus");
        file.ParsedBlocks[0].Tags.Should().BeEquivalentTo(["eins", "zwei", "drei"]);

        file.ParsedBlocks[0].Lines.Should().NotBeNullOrEmpty();

        file.ParsedBlocks[0].Lines![0].Speaker![0].Should().BeOfType<LineNode.TextFragment>().Subject.Text.Should().Be("Michael");
        file.ParsedBlocks[0].Lines![0].Fragments![0].Should().BeOfType<LineNode.TextFragment>().Subject.Text.Should().Be("Hallo Welt!");

        file.ParsedBlocks[0].Lines![1].Speaker![0].Should().BeOfType<LineNode.TextFragment>().Subject.Text.Should().Be("Chris");
        file.ParsedBlocks[0].Lines![1].Fragments![0].Should().BeOfType<LineNode.TextFragment>().Subject.Text.Should().Be("Hi ebenfalls, das geht!");
    }
    
    [Fact]
    public void Parses_SimpleBlock_WithScripts()
    {
        // Arrange
        var text = """
                   title: Grossvaters Haus
                   tags: eins zwei drei
                   -----------
                   Michael: Hallo {jstest1}!
                   Chris: Hi {jstest2}, das geht!
                   {if test}
                   Noch was ohne Name... {jstest3}
                   {else}
                   Simon: Was ist das hier?
                   {endif}
                   =====
                   """;

        var parser = CreateParser(text);
        var tree = parser.file();

        // Act
        var visitor = new LoomDialogVisitor();
        var file = visitor.VisitFile(tree);

        // Assert
        Assert.NotNull(tree);
        Assert.Equal(0, parser.NumberOfSyntaxErrors);

        file.Should().NotBeNull();
    }
}