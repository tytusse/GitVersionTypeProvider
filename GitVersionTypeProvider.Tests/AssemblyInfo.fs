module GitVersionTypeProvider.Tests.AssemblyInfo

open System.Reflection

[<Literal>]
let Version = Git.Version.Brief

[<assembly:AssemblyInformationalVersion(Git.Version.Brief)>]
do ()