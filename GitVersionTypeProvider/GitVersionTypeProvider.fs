namespace GitVersionTypeProvider

//https://github.com/fsprojects/FSharp.TypeProviders.SDK
//https://sergeytihon.com/2016/07/11/f-type-providers-development-tips-not-tricks/
//https://github.com/fsprojects/ProjectScaffold
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open ProviderImplementation.ProvidedTypes
open System.Reflection

[<TypeProvider>]
type Provider() as this =
    inherit TypeProviderForNamespaces()
    let ns = "Git"
    let asm = Assembly.GetExecutingAssembly()

    let createTypes () =
        let myType = ProvidedTypeDefinition(asm, ns, "Version", Some typeof<obj>)
        let myProp = ProvidedLiteralField("Value", typeof<string>, "Hello world")
        myType.AddMember(myProp)
        [myType]

    do
        this.AddNamespace(ns, createTypes())

[<assembly:TypeProviderAssembly>] 
do()