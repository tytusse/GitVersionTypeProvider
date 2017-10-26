namespace GitVersionTypeProvider
//inspired by: https://github.com/Fody/Stamp
//https://github.com/fsprojects/FSharp.TypeProviders.SDK
//https://sergeytihon.com/2016/07/11/f-type-providers-development-tips-not-tricks/
//https://github.com/fsprojects/ProjectScaffold
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open ProviderImplementation.ProvidedTypes
open System.Reflection
open LibGit2Sharp

[<TypeProvider>]
type Provider(cfg:TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces()
    let ns = "Git"
    let asm = Assembly.GetExecutingAssembly()

    let rec findRepositoryRoot root =
        if System.IO.Directory.GetDirectories(root, ".git").Length > 0
        then Some root
        else
            match System.IO.Path.GetDirectoryName root with
            | null -> None
            | root -> findRepositoryRoot root
    
    let formatVersionString root =
        use r = new Repository(root)
        (*
        "sha", r.Head.Tip.Sha 
        "branch", r.Head.TrackedBranch.FriendlyName
        "repo", r.Network.Remotes.[r.Head.RemoteName].Url
        *)
        seq {
            if r.RetrieveStatus().IsDirty then yield "[dirty]"
            yield r.Head.Tip.Sha
            match r.Head.RemoteName with
            | null -> ()
            | x -> 
                yield x
                match r.Network.Remotes |> Seq.tryFind( fun x -> x.Name = r.Head.RemoteName) with
                | Some remote -> yield remote.Url
                | None -> ()
        } |> String.concat(":")
        

    let createTypes () =
        let version =
            match findRepositoryRoot cfg.ResolutionFolder with
            | Some root -> 
                try
                    formatVersionString root
                with x ->
                    raise(exn(sprintf "Failed obtaining version string with error: %s:%s" (x.GetType().Name) x.Message, x))
            | None -> "not versioned"

        let myType = ProvidedTypeDefinition(asm, ns, "Version", Some typeof<obj>)
        let field = ProvidedLiteralField("Value", typeof<string>, version)
        let resFld = ProvidedLiteralField("ResolutionFolder", typeof<string>, cfg.ResolutionFolder)
        
        myType.AddMember(field)
        myType.AddMember(resFld)
        [myType]

    do
        this.AddNamespace(ns, createTypes())

[<assembly:TypeProviderAssembly>] 
do()