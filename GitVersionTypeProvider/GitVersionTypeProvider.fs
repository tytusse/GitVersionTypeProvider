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

type MetaData = {
    Sha:string
    Branch:string
    IsDirty:bool
    RemoteName:string option
    RemoteUrl:string option
    Tags:Tag list
    CommitCount:int
}
with
    member this.Version = 
        seq {
            yield this.Sha
            yield this.Branch
            if this.IsDirty then yield "[dirty]"
        } |> String.concat(":")

    member this.LongVersion = 
        seq {
            if this.IsDirty then yield "[dirty]"
            yield this.Sha
            match this.RemoteName with Some r -> yield r | None -> ()
            match this.RemoteUrl with Some r -> yield r | None -> ()
        } |> String.concat(":")

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
    
    let getRepoData root =
        use r = new Repository(root)
        let remoteName = r.Head.RemoteName |> Option.ofObj
        {
            IsDirty =r.RetrieveStatus().IsDirty
            Sha = r.Head.Tip.Sha
            Branch = r.Head.TrackedBranch.FriendlyName
            RemoteName = remoteName
            RemoteUrl = 
                remoteName 
                |> Option.bind(fun rn -> r.Network.Remotes |> Seq.tryFind( fun r -> r.Name = rn))
                |> Option.map(fun r -> r.Url)
            Tags = r.Tags |> List.ofSeq
            CommitCount = r.Head.Commits |> Seq.length
        }
        

    let createTypes () =
        let metaData =
            findRepositoryRoot cfg.ResolutionFolder
            |> Option.map(fun root ->
                try getRepoData root
                with x ->
                    raise(exn(sprintf "Failed obtaining version string with error: %s:%s" (x.GetType().Name) x.Message, x))
            )

        match metaData with
        | Some metaData ->
            let paremeterless = ProvidedTypeDefinition(asm, ns, "General", Some typeof<obj>)
            [
                ProvidedLiteralField("LongVersion", typeof<string>, metaData.LongVersion)
                ProvidedLiteralField("BriefVersion", typeof<string>, metaData.Version)
                ProvidedLiteralField("Sha", typeof<string>, metaData.Sha)
                ProvidedLiteralField("IsDirty", typeof<bool>, metaData.IsDirty)
                ProvidedLiteralField("Branch", typeof<string>, metaData.Branch)
                ProvidedLiteralField("RemoteName", typeof<string>, metaData.RemoteName |> Option.defaultValue "")
                ProvidedLiteralField("RemoteUrl", typeof<string>, metaData.RemoteUrl |> Option.defaultValue "")
                ProvidedLiteralField("ResolutionFolder", typeof<string>, cfg.ResolutionFolder)
                ProvidedLiteralField("Tags", typeof<string>, metaData.Tags |> List.map(fun t -> t.FriendlyName)|> String.concat "|")
            ]
            |> List.iter paremeterless.AddMember

            let assemblyVersion = ProvidedTypeDefinition(asm, ns, "AssemblyVersion", Some typeof<obj>)
            assemblyVersion.AddXmlDoc("This type will generate proper assembly version, by concatenating provided minor, major and revision with build represented by commit count")
            let prefix = 
                ["Major";"Minor";"Revision"]
                |> List.map (fun n -> ProvidedStaticParameter(n, typeof<int>))

            assemblyVersion.DefineStaticParameters(prefix, (fun typeName parameterValues ->
                match parameterValues with 
                | [| :? int as major; :? int as minor; :? int as revision |] ->
                    let ty = ProvidedTypeDefinition(asm, ns, typeName, baseType = Some typeof<obj>)
                    let ver = sprintf "%d.%d.%d.%d" major minor revision metaData.CommitCount
                    ty.AddMember(ProvidedLiteralField("Value", typeof<string>, ver))
                    ty
                | x -> failwithf "unsupported parameter list: %A" x
                )
            )

            [paremeterless;assemblyVersion]
        | None -> []

    do
        this.AddNamespace(ns, createTypes())

[<assembly:TypeProviderAssembly>] 
do()