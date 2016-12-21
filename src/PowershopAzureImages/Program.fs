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
    open Suave.Successful
    open Suave.RequestErrors
    open Suave.Filters
    open Suave.Operators
    open System.Net
    open System

    open Suave.Swagger
    open Rest
    open FunnyDsl
    open Swagger

    let now : WebPart =
        fun (x : HttpContext) ->
            async {
            // The MODEL helper checks the "Accept" header 
            // and switches between XML and JSON format
            return! MODEL DateTime.Now x
            }

    [<CLIMutable>] 
    type Pet =
        { 
          Id:int
          Name:string
          Category:PetCategory }
    and [<CLIMutable>] PetCategory = 
        { Id:int
          Name:string }

    let uploadImage =
        let upload r = 
            //let s = match r[0] with 
            //          | (s, None) -> None
            //          | (_ ,Some x) -> x
                
            match r.files with
            | [] -> "No filename supplied !!!!!!!" |> BAD_REQUEST 
            | x::_ -> sprintf "%s %s %s" x.fieldName x.fileName x.tempFilePath  |> OK 
        request upload 


    let imageApi = 
        swagger {
            // syntax 1
            for route in getting (simpleUrl "/time" |> thenReturns now) do
                yield description Of route is "What time is it ?"

            for route in posting <| simpleUrl "/pictures" |> thenReturns uploadImage do
                yield description Of route is "Post an image"
                yield route |> addResponse 200 "Returns the URL of the created image" None
                yield parameter "File to upload" postOf route (fun p -> { p with Name = "Files"; Type = (Some typeof<System.IO.File>); In=FormData })
        }

    [<EntryPoint>]
    let main argv =
        let config = defaultConfig
        let config =
            match IISHelpers.httpPlatformPort with
            | Some port ->
                { config with
                    bindings = [ HttpBinding.mkSimple HTTP "127.0.0.1" port ] }
            | None -> config

        

        let routes = 
            choose [
                GET >=> path "/" >=> (Successful.OK "Welcome to Powershop AZURE Images API")
                POST >=> path "/pictures" >=> uploadImage
                
                RequestErrors.NOT_FOUND "Found no handlers"
        ]        

        startWebServer config routes
        0