# GitVersionTypeProvider
Type provider that generates a simple class which exposes git version (to be used in assembly attributes for example).

Inspired by similar idea: https://github.com/Fody/Stamp where I encountered issues with 

- programs not compiling well sometimes
- compile time getting elongated....

This is proof of concept for using type providers as versioning tool that can get version from git and put it as assembly metadata.

Goal is to use it like this:

- use paket or nuget to fetch project dll and to generate type with version on the fly
- use it in F# directly like this: `[<assembly:AssemblyInformationalVersion(Git.General.BriefVersion)>]`
- or like this: `[<assembly:AssemblyInformationalVersion(Git.General.LongVersion)>]`
- also use it with AssemblyVersion which requires `major.minor.rev.build` format: `[<assembly: AssemblyVersion(Git.AssemblyVersion<1,0,0>.Value)>]`
- alternatively, define F# proxy dll, then define literal in some public module, then use it in C# or VB.Net or whatever .NET lang, like this:

```FSharp
module MyLibrary

[<Literal>]
let Version = Git.General.BriefVersion

[<Literal>]
let AsmVersion = Git.AssemblyVersion<1,0,0>.Value
```

and then in C#:

```csharp
using System.Reflection;
[assembly:AssemblyFileVersion(MyLibrary.Version)]
[assembly:AssemblyVersion(MyLibrary.AsmVersion)]
```

# Git.General: Accessing some general git info
All fields that are generated in `Git.General` class are:
- `Sha:string` - commit sha
- `Tags:string` - list of tags separated by "|" char
- `IsDirty:bool` - set to `true` if some uncomited changes exist in workdir
- `Branch:string` - name of current branch, i.e.: "origin/master"
- `RemoteName:string` - name of remote ref, i.e. "origin"
- `RemoteUrl:string` - url of remote branch, i.e. "git@github.com:tytusse/GitVersionTypeProvider.git"
- `LongVersion:string` - long version string in format: "Sha:RemoteUrl" with optional "dirty" prefix
- `BriefVersion:string` - short version string in format: "Sha:BranchName"  with optional "dirty" suffix
- `CommitCount:int` - number of commits in current branch

# Git.AssemblyVersion<...>: generating proper AssemblyVersion string
`Git.AssemblyVersion<...>` type allows generating AssemblyVersion in `major.minor.rev.build` format, where first 3 numbers must be provided in code as compile time vars, while for the last one ("build") git branch commit count is used, for example:

```FSharp
[<assembly: AssemblyVersion(Git.AssemblyVersion<1,2,3>.Value)>]
```

# Issues
## Nuget package not yet available.
The provider uses/requires 3rd party libs to read git information, namely *LibGit2Sharp*, which is a bit tricky as it uses native dll.  Currently nuget does not work with it "out of the box", as type provider needs all its dependencies ad design time and thus it needs ale it's dependencies be available also at design time. Normal flow with nuget is to compile assemblies so most of dependencies are not  "glued" until compilation (i.e. dlls are in different folders)

To solve this, two paths may be selected:
- bundle external libs directly with main provider lib, perhaps with nuget install script
- ilmerge them - this may not work as native dlls are an unknown here.... (to be researched)

# References
- https://github.com/Fody/Stamp - allows to embedd git version in post-commit style (Mono.Cecil) - main inspiration for this exercise
- https://github.com/fsprojects/FSharp.TypeProviders.SDK - the starting point for type providers
- https://sergeytihon.com/2016/07/11/f-type-providers-development-tips-not-tricks/ - good advices for type providers
- https://fsprojects.github.io/Paket/getting-started.html - paket, which I used for the 1st time in this project and I already see adventages
