#I @"packages/FAKE/tools/"
#r @"FakeLib.dll"

open Fake
open Fake.Azure
open System
open System.IO

Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
let solutionFile = "PowershopAzureImages.sln"

Target "BuildSolution" (fun _ ->
    solutionFile
    |> MSBuildHelper.build (fun defaults ->
        { defaults with
            Verbosity = Some Minimal
            Targets = [ "Build" ]
            Properties = [ "Configuration", "Release"
                           "OutputPath", Kudu.deploymentTemp ] })
    |> ignore)

Target "Deploy" Kudu.kuduSync

"BuildSolution"
==> "Deploy"


RunTargetOrDefault "Deploy"