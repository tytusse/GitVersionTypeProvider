module GitVersionTypeProvider.Tests.AssemblyInfo

open System.Reflection

[<Literal>]
let Version = Git.Version.Value

[<assembly:AssemblyInformationalVersion(Git.Version.Value)>]
do ()