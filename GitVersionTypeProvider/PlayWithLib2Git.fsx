//http://christoph.ruegg.name/blog/loading-native-dlls-in-fsharp-interactive.html
//https://github.com/libgit2/libgit2sharp
module Kernel =
    open System.Runtime.InteropServices
    [<DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)>]
    extern System.IntPtr LoadLibrary(string lpFileName);

type Path = System.IO.Path

Kernel.LoadLibrary(Path.Combine(__SOURCE_DIRECTORY__, @"..\packages\LibGit2Sharp.NativeBinaries\runtimes\win7-x86\native\git2-15e1193.dll"))

#r @"..\packages\LibGit2Sharp\lib\net40\LibGit2Sharp.dll"

open LibGit2Sharp

do
    use r = new Repository(Path.GetDirectoryName(__SOURCE_DIRECTORY__))
    [
        "sha", r.Head.Tip.Sha 
        "branch", r.Head.TrackedBranch.FriendlyName
        "repo", r.Network.Remotes.[r.Head.RemoteName].Url
    ]
    |> List.iter (fun (n,v) -> printfn "%s=%s" n v)
    