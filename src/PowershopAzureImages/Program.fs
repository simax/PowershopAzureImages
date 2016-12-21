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
    open System.IO

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
        let rootPath =
            match Environment.GetEnvironmentVariable("WEROOT_PATH") with
            | null -> "/users/simonlomax/temp/images"
            | value -> value

        let upload r = 
            let moveFiles2Root srcFile destFile =
                let destination = Path.Combine(rootPath, destFile)
                if File.Exists(destination) then File.Delete(destination)
                System.IO.File.Move(srcFile, destination)    
                destination

            match r.files with
            | [] -> "No filename supplied !!!!!!!" |> BAD_REQUEST 
            | x::_ -> moveFiles2Root x.tempFilePath x.fileName 
                      |> sprintf "Moved: %s to %s " x.tempFilePath 
                      |> OK 

        request upload 

    let imageApi = 
        swagger {
            // syntax 1
            for route in getting (simpleUrl "/time" |> thenReturns now) do
                yield description Of route is "What time is it ?"

            for route in getOf (path "/time2" >=> now) do
                yield urlTemplate Of route is "/time2"
                yield description Of route is "What time is it 2 ?"

            for route in postOf (path "/pictures" >=> uploadImage) do
                yield urlTemplate Of route is "/pictures"
                yield description Of route is "Post an image"
                yield route |> addResponse 200 "Returns the URL of the created image" None
                yield parameter "File to upload" postOf route (fun p -> { p with Name = "uploadedImage"; Type = (Some typeof<System.IO.File>); In=FormData })
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

        startWebServer config imageApi.App
        0