namespace ScriptService;

/// <summary>
/// Represents a container for sharing key-value data across components or operations.
/// </summary>
/// <remarks>Use this class to store and retrieve shared state information using string keys. The contained data
/// can be accessed and modified by multiple consumers, enabling coordination or data passing between different parts of
/// an application. Thread safety for concurrent access to the data is not guaranteed; callers should implement their
/// own synchronization if required.</remarks>
public sealed class SharedState
{
    public Dictionary<string, object> Data { get; } = [];
}
