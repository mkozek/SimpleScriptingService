namespace ScriptService;

public sealed class ScriptResult
{
    public bool IsSuccess { get; }
    public object? ReturnValue { get; }
    public string? ErrorValue { get; }

    private ScriptResult(bool success, object? value, string? error)
    {
        IsSuccess = success;
        ReturnValue = value;
        ErrorValue = error;
    }

    public static ScriptResult Success(object? value) =>
        new(true, value, null);

    public static ScriptResult Error(string error) =>
        new(false, null, error);
}
