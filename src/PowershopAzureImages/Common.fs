namespace Common

type Result<'TSuccess> = 
    | Success of 'TSuccess
    | Failure of string

module Helpers = 

    open System.IO
    open Newtonsoft.Json
    open Newtonsoft.Json.Serialization
    open Suave
    open Suave.Operators
    let destinationPath imageFileName imageSize = 
        let fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFileName)
        let fileExtension = Path.GetExtension(imageFileName)
        sprintf "images/%s-[%s]%s" fileNameWithoutExtension imageSize fileExtension
    let JSON wp v =
        let settings = new JsonSerializerSettings()
        settings.ContractResolver <-
            new CamelCasePropertyNamesContractResolver()

        JsonConvert.SerializeObject(v, settings)
        |> wp
        >=> Writers.setMimeType "application/json; charset=utf-8"