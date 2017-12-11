module GitVersionTypeProvider.Tests.AssemblyInfo

open System.Reflection

[<Literal>]
let Version = Git.General.BriefVersion

[<Literal>]
let AsmVersion = Git.AssemblyVersion<1,0,0>.Value

[<assembly:AssemblyInformationalVersion(Git.General.BriefVersion)>]
[<assembly: AssemblyVersion(Git.AssemblyVersion<1,0,0>.Value)>]
do ()