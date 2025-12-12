# SimpleScriptingService  
A lightweight C# scripting engine built on Roslyn, designed for embedding safe, extensible, and stateful scripting into your applications.

## ✅ Features  
- Execute C# scripts at runtime  
- Strongly‑typed globals  
- Shared state across script executions  
- Structured results (return value, exception, diagnostics)  
- Minimal dependencies 
- MIT licensed  

## 📦 Installation  

```bash
dotnet add package SimpleScriptingService
```

Or via the NuGet Package Manager:

```
Install-Package SimpleScriptingService
```

## 🚀 Quick Start

### Create the engine
```csharp
var engine = new ScriptEngine();
```

### Execute a simple script
```csharp
var result = await engine.ExecuteAsync("1 + 2");
Console.WriteLine(result.ReturnValue); // 3
```

## 📁 API Overview

### `ScriptEngine`
- Executes scripts asynchronously  
- Accepts optional globals  
- Provides access to shared state  

### `ScriptGlobals`
- Strongly‑typed object exposed to scripts  
- Extend this class to expose your own API surface  
- Provides `Log` method 

### `SharedState`
- `Dictionary<string,object>` storage shared across script runs  

### `ScriptResult`
- `ReturnValue`   
- `ErrorValue`
- `IsSuccess`



## 📝 License  
MIT License — free for commercial and open‑source use.
