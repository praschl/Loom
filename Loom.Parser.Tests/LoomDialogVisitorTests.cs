using Antlr4.Runtime;
using FluentAssertions;
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
        file.ParsedBlocks[0].Lines[0].Speaker.Should().Be("Michael");
        file.ParsedBlocks[0].Lines[1].Speaker.Should().Be("Chris");
    }
}