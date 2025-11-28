using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Loom.Parser.Tests;

public class ThrowingErrorListener : BaseErrorListener
{
    public static readonly ThrowingErrorListener Instance = new ThrowingErrorListener();

    public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new ParseCanceledException($"line {line}:{charPositionInLine} {msg}");
    }
}
