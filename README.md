# GitVersionTypeProvider
Type provider that generates a simple class which exposes git version (to be used in assembly attributes for example).

Inspired by similar idea: https://github.com/Fody/Stamp where I encountered issues with 

- programs not compiling well sometimes
- compile time getting elongated....

This is proof of concept for using type providers as versioning tool that can get version from git and put it as assembly metadata.

Goal is to use it like this:

- use paket or nuget to fetch project dll and to generate type with version on the fly
- use it in F# directly like this: `[<assembly:AssemblyInformationalVersion(Git.Version.Value)>]`
- alternatively, define F# proxy dll, then define literal in some public module, then use it in C# or VB.Net or whatever .NET lang, like this:

```FSharp
module MyLibrary

[<Literal>]
let Version = Git.Version.Value
```

and then in C#:

```csharp
using System.Reflection;
[assembly:AssemblyInformationalVersion(MyLibrary.Version)]
```
# Issues
It uses/requires 3rd party libs to read git information, namely *LibGit2Sharp*, which is a bit tricky as it uses native dll.

## Constants/Literals to the rescue
The `Git.Version.Value` field is a **const string** field. 
Now, constants/literals in .NET are **inlined** and I have a feeling that this might allow to discard 3rd party libs at runtime.

Let's consider the following approach

- F# project called "Version" defines literal in some module and directly references "git version" type provider
- other projects reference "Version" dll and use it's literal. They, however, do not reference type provider directly. 
During compilation, type provider dlls should **not** be copied to *other projects* bin dirs and at the same time - 
the projects should work without issues.

This is a bit tricky, but when you think about it, you may even start to consider it as *elegant* :-)
