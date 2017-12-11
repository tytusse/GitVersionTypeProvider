#r @"..\GitVersionTypeProvider\bin\Release\GitVersionTypeProvider.dll"

open System.Reflection

[<assembly: AssemblyFileVersion(Git.Version.Brief)>]
do ()

printfn "%s" Git.Version.ResolutionFolder