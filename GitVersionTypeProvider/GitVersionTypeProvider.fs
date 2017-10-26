module GitVersionTypeProvider

//https://github.com/fsprojects/FSharp.TypeProviders.SDK
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations

[<TypeProvider>]
type Provider() =
    inherit ProviderImplementation.ProvidedTypes.TypeProviderForNamespaces()

    [<assembly:TypeProviderAssembly>] 
    do()