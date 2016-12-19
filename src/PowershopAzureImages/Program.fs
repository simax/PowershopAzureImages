module PowershopAzureImages

open Suave
open Suave.Filters
open Suave.Operators
open System.Net
open System

[<EntryPoint>]
let main argv =

    // let config =
    //         { defaultConfig with
    //             bindings = [ HttpBinding.mk HTTP IPAddress.Loopback (uint16 port) ]
    //             listenTimeout = TimeSpan.FromMilliseconds 3000. }

    startWebServer defaultConfig (Successful.OK "Hello World!")
    0