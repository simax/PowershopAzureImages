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

    open Suave.Swagger
    open Rest
    open FunnyDsl
    open Swagger

    open AzureStorageHelpers

    let now : WebPart =
        fun (x : HttpContext) ->
            async {
            // The MODEL helper checks the "Accept" header 
            // and switches between XML and JSON format
            return! MODEL DateTime.Now x
            }

    type ImageInfo = {
        sourceFile : string
        shopId: string
        product: string
        imageSize: string
    }        

    type Result<'TSuccess,'TFailure> = 
        | Success of 'TSuccess
        | Failure of 'TFailure

    let uploadImage =
        // Url: /images?shopid=shop001&product=balloon&imageSize=78x78

        let imageInfo = { sourceFile = ""; shopId = ""; product = ""; imageSize = ""}
    
        let validateSourceFileExists (req:HttpRequest) = 
            match req.files with
            | [] -> Failure "No file(s) were supplied" 
            | h :: _ -> Success (req, { imageInfo with sourceFile = h.tempFilePath })

        let validateShopIdExists (req:HttpRequest, imageInfo) =    
            match req.queryParam "shopid" with
            | Choice1Of2 shopId -> Success (req, { imageInfo with shopId = shopId})
            | Choice2Of2 _ -> Failure "ShopId not supplied" 
            
        let validateProductExists (req:HttpRequest, imageInfo) =    
            match req.queryParam "product" with
            | Choice1Of2 product -> Success (req, { imageInfo with product = product })
            | Choice2Of2 _ -> Failure "Product not supplied" 

        let validateImageSizeExists (req:HttpRequest, imageInfo) =    
            match req.queryParam "imageSize" with
            | Choice1Of2 imageSize -> Success (req, { imageInfo with imageSize = imageSize })
            | Choice2Of2 _ -> Failure "ImageSize not supplied" 

        let bind switchFn input = 
            match input with
            | Success s -> switchFn s
            | Failure f -> Failure f

        let (>>=) input switchFn = bind switchFn input 

        let validateImageInfo imageInfo = 
            imageInfo 
            |> validateSourceFileExists
            >>= validateShopIdExists  // ... so use "bind pipe". Again the result is a two track output
            >>= validateProductExists
            >>= validateImageSizeExists


            // uploadToAzurecontainer h.tempFilePath (Path.Combine("products/images/", h.fileName))
            //             |> sprintf "Moved: %s to %s " h.tempFilePath 
            //             |> OK 
            

        let upload req = 

            let uploadToAzure (imageInfo:ImageInfo) =
                let container = GetShopContainer imageInfo.shopId
                let sourcePath = imageInfo.sourceFile
                let destinationPath = sprintf "images/%s-[%s].jpg" imageInfo.product imageInfo.imageSize
                AzureStorageHelpers.uploadFile container sourcePath destinationPath 

            let uploadIfValid data =    
                match data with
                | Success (r, imageInfo) -> uploadToAzure imageInfo
                | Failure error -> error 
    
            // let convertToString x = 
            //     match x with 
            //     | Success (r, imageInfo) -> sprintf "%A" imageInfo
            //     | Failure f -> f

            validateImageInfo req 
            // |> convertToString |> OK
            |> uploadIfValid 
            |> OK


        request upload 

    let imageApi = 
        swagger {
            // for route in getting (simpleUrl "/time" |> thenReturns now) do
            //     yield description Of route is "What time is it ?"

            // for route in getOf (path "/time2" >=> now) do
            //     yield urlTemplate Of route is "/time2"
            //     yield description Of route is "What time is it 2 ?"

            for route in postOf (path "/pictures" >=> uploadImage) do
                yield urlTemplate Of route is "/pictures"
                yield description Of route is "Post an image"
                yield route |> addResponse 200 "Returns the URL of the created image" None
                yield parameter "File to upload" postOf route (fun p -> { p with Name = "uploadedImage"; Type = (Some typeof<System.IO.File>); In=FormData })
        }

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