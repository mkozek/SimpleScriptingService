using Microsoft.Extensions.Logging;
namespace ScriptService;

public sealed class ScriptGlobals
{
    private ILogger? _logger;
    public SharedState State { get; }

    public ScriptGlobals(SharedState state, ILogger? logger)
    {
        State = state;
    }

    public void Log(string message)
    {
        _logger?.LogInformation("[SCRIPT] {Message}", message);
    }
}
