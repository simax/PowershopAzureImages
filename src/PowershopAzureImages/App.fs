﻿namespace PowershopAzureImages

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


    let proceedIfValid req imageInfo validator proceed =
        match validator req imageInfo with 
        | Success (_, imageInfo) -> proceed imageInfo
        | Failure error -> error |> BAD_REQUEST 

    let deleteImage =

        let delete req = 
            let deleteFromAzure (imageInfo:ImageInfo) =
                let container = GetShopContainer imageInfo.shopId
                let imagePath = sprintf "images/%s-[%s].jpg" imageInfo.product imageInfo.imageSize

                let isDeleted = AzureStorageHelpers.deleteFile container imagePath
                if isDeleted then GONE "Image removed" else BAD_REQUEST "Unable to delete image"

            let imageInfo = { sourceFile = ""; shopId = ""; product = ""; imageSize = ""}        
            proceedIfValid req imageInfo validateImageDeletionInfo deleteFromAzure        
           
        request delete


    let uploadImage =
        // Url: /images?shopid=shop001&product=balloon&imageSize=78x78

        let upload req = 
            let uploadToAzure (imageInfo:ImageInfo) =
                let container = GetShopContainer imageInfo.shopId
                let sourcePath = imageInfo.sourceFile
                let destinationPath = sprintf "images/%s-[%s].jpg" imageInfo.product imageInfo.imageSize
                AzureStorageHelpers.uploadFile container sourcePath destinationPath 
                |> CREATED

            let imageInfo = { sourceFile = ""; shopId = ""; product = ""; imageSize = ""}        
            proceedIfValid req imageInfo validateImageCreationInfo uploadToAzure        
           
        request upload 

    [<EntryPoint>]

    let main argv =

        // Maximum image size is 5 MB becuase maxContentLength od Post data is 5MB   
        let config = { defaultConfig with maxContentLength = 5000000 } 
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
                DELETE >=> path "/images" >=> deleteImage


                RequestErrors.NOT_FOUND "Found no handlers"
        ]        

        startWebServer config routes
        0