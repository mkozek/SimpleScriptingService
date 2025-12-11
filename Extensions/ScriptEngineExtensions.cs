namespace ScriptService.Extensions;

public static class ScriptEngineExtensions
{
    extension(ScriptEngine engine)
    {
        /// <summary>
        /// This extension adds namespaces to the script engine's import list.
        /// use <see cref="nameof"/> to provide namespace names safely.
        /// </summary>
        /// 
        /// <returns></returns>
        public ScriptEngine WithImports(params string?[] namespaces)
        {
            engine.AddImports(namespaces);
            return engine;
        }
        public ScriptEngine WithReferences(params Span<Type> types)
        {
            foreach (var type in types)
            {
                engine.AddReference(type);
            }
            return engine;
        }
    }

}
