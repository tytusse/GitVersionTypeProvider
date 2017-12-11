#r @"..\GitVersionTypeProvider\bin\Release\GitVersionTypeProvider.dll"

open System.Reflection

[<assembly: AssemblyFileVersion(Git.General.BriefVersion)>]
[<assembly: AssemblyVersion(Git.AssemblyVersion<1,0,0>.Value)>]
do ()

printfn "%s" Git.General.ResolutionFolder