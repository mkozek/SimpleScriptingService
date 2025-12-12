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


    /// <summary>
    /// Initializes a new instance of the ScriptEngine class with the specified global variables and optional default
    /// imports.
    /// </summary>
    /// <remarks>If addDefaultImports is set to true, the engine automatically imports namespaces such as
    /// System, System.Linq, System.Text, and System.Collections.Generic, making their types available in scripts
    /// without requiring explicit import statements.</remarks>
    /// <param name="globals">An object containing global variables and context to be made available to scripts executed by the engine. 
    /// If null default ScriptGlobals implementation is provided</param>
    /// <param name="addDefaultImports">true to add a set of commonly used default namespace imports to the script environment; otherwise, false.</param>
    public static ScriptEngine ScriptEngineWithGlobals(ScriptGlobals globals, bool addDefaultImports = true) =>
        new(globals, addDefaultImports);

    /// <summary>
    /// Creates a new instance of the script engine configured with the specified shared state, optional default
    /// imports, and an optional logger.
    /// </summary>
    /// <param name="state">The shared state to be used by the script engine. Cannot be null.</param>
    /// <param name="addDefaultImports">true to add the default set of imports to the script engine; otherwise, false. The default is true.</param>
    /// <param name="logger">An optional logger used to record script execution events. If null, no logging is performed.</param>
    /// <returns>A new ScriptEngine instance initialized with the provided shared state and configuration.</returns>
    public static ScriptEngine ScriptEngineWithState(SharedState state, bool addDefaultImports = true, ILogger? logger = null) =>
        new(new ScriptGlobals(state, logger), addDefaultImports);


    private ScriptEngine(ScriptGlobals? globals, bool addDefaultImports = true)
    {
        _globals = globals ?? new ScriptGlobals(null);

        _options = ScriptOptions.Default
            .AddReferences(
                typeof(object).Assembly,
                typeof(ScriptGlobals).Assembly
            );
        if (addDefaultImports)
            _options = _options.AddImports(
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

    public void AddReference(Type type)
    {
        _options = _options.AddReferences(type.Assembly);
    }
    public void AddImports(params string[] @namespaces)
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