using System.Text;
using Xunit.Abstractions;

namespace Loom.Parser.Tests;

public class Forward : TextWriter
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly TextWriter _oldConsole;

    public static IDisposable ToConsole(ITestOutputHelper console)
    {
        return new Forward(console);
    }

    private Forward(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _oldConsole = Console.Out;
        Console.SetOut(this);
    }
    
    public override Encoding Encoding { get; } = Encoding.UTF8;

    public override void WriteLine(string? message)
    {
        if (_closed)
            throw new InvalidOperationException("Already closed the forwarder");

        _outputHelper.WriteLine(message);
    }

    private bool _closed;
    
    public override void Close()
    {
        base.Close();
            
        Console.SetOut(_oldConsole);
        _closed = true;
    }
}