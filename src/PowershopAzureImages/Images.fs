namespace PowershopAzureImages
open System
open Suave
open Common

module Images = 

    type ImageInfo = {
        sourceFile : string
        shopId: string
        imageFileName: string
        imageSize: string
    }    

    type ImageResource = { image: string }

    // Validation 

    let validateSourceFileExists (req:HttpRequest, imageInfo) = 
        match req.files with
        | [] -> Failure "No file(s) were supplied" 
        | h :: _ -> Success (req, { imageInfo with sourceFile = h.tempFilePath })

    let validateShopIdExists (req:HttpRequest, imageInfo) =    
        match req.queryParam "shopid" with
        | Choice1Of2 shopId -> Success (req, { imageInfo with shopId = shopId})
        | Choice2Of2 _ -> Failure "shopId not supplied" 
        
    let validateImageFileNameExists (req:HttpRequest, imageInfo) =    
        match req.queryParam "image" with
        | Choice1Of2 imageFileName -> Success (req, { imageInfo with imageFileName = imageFileName })
        | Choice2Of2 _ -> Failure "Image not supplied" 

    let validateImageSizeExists (req:HttpRequest, imageInfo) =    
        match req.queryParam "size" with
        | Choice1Of2 imageSize -> Success (req, { imageInfo with imageSize = imageSize })
        | Choice2Of2 _ -> Failure "size not supplied" 

    let bind switchFn input = 
        match input with
        | Success s -> switchFn s
        | Failure f -> Failure f

    let (>>=) input switchFn = bind switchFn input 

    let validateImageCreationInfo req imageInfo = 
        (req, imageInfo)
        |> validateSourceFileExists
        >>= validateShopIdExists  
        >>= validateImageFileNameExists
        >>= validateImageSizeExists

    let validateImageDeletionInfo req imageInfo = 
        (req, imageInfo)  
        |> validateShopIdExists  
        >>= validateImageFileNameExists
        >>= validateImageSizeExists

 