using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;

namespace ScriptService;

public sealed class ScriptEngine
{
    private ScriptOptions _options;
    private readonly ScriptGlobals _globals;

    // Keeps state between script executions
    private ScriptState<object>? _state;

    public static ScriptEngine ScriptEngineWithGlobals(ScriptGlobals globals) =>
        new(globals);

    public static ScriptEngine ScriptEngineWithState(SharedState state, ILogger? logger = null) =>
        new(new ScriptGlobals(state, logger));

    private ScriptEngine(ScriptGlobals globals)
    {
        _globals = globals;

        _options = ScriptOptions.Default
            .AddReferences(
                typeof(object).Assembly,
                typeof(ScriptGlobals).Assembly,
                typeof(System.Text.Json.JsonDocument).Assembly
            )
            .AddImports(
                "System",
                "System.Linq",
                "System.Text",
                "System.Collections.Generic"
            );
    }

    public void ClearState()
    {
        _state = null;
    }

    internal void AddReference(Type type)
    {
        _options = _options.AddReferences(type.Assembly);
    }
    internal void AddImports(params string[] @namespaces)
    {
        _options = _options.AddImports(@namespaces);
    }
    /// <summary>
    /// Executes a script snippet. State is preserved across calls.
    /// </summary>
    public async Task<ScriptResult> ExecuteAsync(string code, CancellationToken ct = default)
    {
        try
        {
            if (_state == null)
            {
                _state = await CSharpScript.RunAsync(code, _options, _globals, cancellationToken: ct);
            }
            else
            {
                _state = await _state.ContinueWithAsync(code, cancellationToken: ct);
            }

            return ScriptResult.Success(_state.ReturnValue);
        }
        catch (CompilationErrorException ex)
        {
            return ScriptResult.Error(string.Join(Environment.NewLine, ex.Diagnostics));
        }
        catch (Exception ex)
        {
            return ScriptResult.Error(ex.ToString());
        }
    }

    /// <summary>
    /// Loads a script file and executes it.
    /// </summary>
    public async Task<ScriptResult> ExecuteFileAsync(string path, CancellationToken ct = default)
    {
        if (!File.Exists(path))
            return ScriptResult.Error($"Script file not found: {path}");

        var code = await File.ReadAllTextAsync(path, ct);
        return await ExecuteAsync(code, ct);
    }
}