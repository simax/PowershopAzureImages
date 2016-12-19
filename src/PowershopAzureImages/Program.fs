module PowershopAzureImages

open Suave
open Suave.Filters
open Suave.Operators

[<EntryPoint>]
let main argv =

    startWebServer defaultConfig (Successful.OK "Hello World!")
    0