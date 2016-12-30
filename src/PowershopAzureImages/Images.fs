namespace PowershopAzureImages
open System
open Suave
open Common

module Images = 

    type ImageInfo = {
        sourceFile : string
        shopId: string
        product: string
        imageSize: string
    }    

    // let (|IgnoreCase|_|) (str:string) arg = 
    //     if String.Compare(str, arg, StringComparison.OrdinalIgnoreCase) = 0
    //         then Some() else None

    let imageInfo = { sourceFile = ""; shopId = ""; product = ""; imageSize = ""}

    let validateSourceFileExists (req:HttpRequest) = 
        match req.files with
        | [] -> Failure "No file(s) were supplied" 
        | h :: _ -> Success (req, { imageInfo with sourceFile = h.tempFilePath })

    let validateShopIdExists (req:HttpRequest, imageInfo) =    
        match req.queryParam "shopid" with
        | Choice1Of2 shopId -> Success (req, { imageInfo with shopId = shopId})
        | Choice2Of2 _ -> Failure "shopId not supplied" 
        
    let validateProductExists (req:HttpRequest, imageInfo) =    
        match req.queryParam "product" with
        | Choice1Of2 product -> Success (req, { imageInfo with product = product })
        | Choice2Of2 _ -> Failure "product not supplied" 

    let validateImageSizeExists (req:HttpRequest, imageInfo) =    
        match req.queryParam "size" with
        | Choice1Of2 imageSize -> Success (req, { imageInfo with imageSize = imageSize })
        | Choice2Of2 _ -> Failure "size not supplied" 

    let bind switchFn input = 
        match input with
        | Success s -> switchFn s
        | Failure f -> Failure f

    let (>>=) input switchFn = bind switchFn input 

    let validateImageInfo imageInfo = 
        imageInfo 
        |> validateSourceFileExists
        >>= validateShopIdExists  
        >>= validateProductExists
        >>= validateImageSizeExists
