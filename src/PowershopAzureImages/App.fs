namespace PowershopAzureImages

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

module Api =

    open Suave
    open Suave.Successful
    open Suave.RequestErrors
    open Suave.Filters
    open Suave.Operators
    open System.Net
    open System
    open System.IO
    open Common
    open Images

    open AzureStorageHelpers

    let uploadImage =
        // Url: /images?shopid=shop001&product=balloon&imageSize=78x78

        let upload req = 

            let uploadToAzure (imageInfo:ImageInfo) =
                let container = GetShopContainer imageInfo.shopId
                let sourcePath = imageInfo.sourceFile
                let destinationPath = sprintf "images/%s-[%s].jpg" imageInfo.product imageInfo.imageSize
                AzureStorageHelpers.uploadFile container sourcePath destinationPath 

            let uploadIfValid data =    
                match data with
                | Success (r, imageInfo) -> uploadToAzure imageInfo |> CREATED
                | Failure error -> error |> BAD_REQUEST 
    
            // let convertToString x = 
            //     match x with 
            //     | Success (r, imageInfo) -> sprintf "%A" imageInfo
            //     | Failure f -> f

            validateImageInfo req 
            // |> convertToString |> OK
            |> uploadIfValid 
            
        request upload 

    [<EntryPoint>]
    let main argv =

        let config = { defaultConfig with maxContentLength = 5000000 } // 5 MB
        let config =
            match IISHelpers.httpPlatformPort with
            | Some port ->
                { config with
                    bindings = [ HttpBinding.createSimple HTTP "127.0.0.1" port ]
                }
            | None -> config 

        

        let routes = 
            choose [
                GET >=> path "/" >=> (Successful.OK "Welcome to Powershop AZURE Images API")
                POST >=> path "/images" >=> uploadImage
                
                RequestErrors.NOT_FOUND "Found no handlers"
        ]        

        startWebServer config routes
        0