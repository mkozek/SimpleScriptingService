using Microsoft.Extensions.Logging;
namespace ScriptService;

public class ScriptGlobals
{
    private ILogger? _logger;
    public SharedState State { get; }

    public ScriptGlobals(SharedState state, ILogger? logger)
    {
        State = state;
    }
    public ScriptGlobals(ILogger? logger)
    {
        State = new SharedState();
    }

    public void Log(string message)
    {
        _logger?.LogInformation("[SCRIPT] {Message}", message);
    }
}
