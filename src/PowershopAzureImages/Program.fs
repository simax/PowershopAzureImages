module IISHelpers =
    open System

    /// Port specified by IIS HttpPlatformHandler
    let httpPlatformPort =
        match Environment.GetEnvironmentVariable("HTTP_PLATFORM_PORT") with
        | null -> None
        | value ->
            match Int32.TryParse(value) with
            | true, value -> Some value
            | false, _ -> None


module PowershopAzureImages =

    open Suave
    open Suave.Filters
    open Suave.Operators
    open System.Net
    open System

    [<EntryPoint>]
    let main argv =
        let config = defaultConfig
        let config =
            match IISHelpers.httpPlatformPort with
            | Some port ->
                { config with
                    bindings = [ HttpBinding.mkSimple HTTP "127.0.0.1" port ] }
            | None -> config

        startWebServer config (Successful.OK "Hello World!")
        0