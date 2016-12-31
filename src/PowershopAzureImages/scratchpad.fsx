#r "../../packages/WindowsAzure.Storage/lib/net45/Microsoft.WindowsAzure.Storage.dll"
#r "../../packages/Microsoft.Azure.KeyVault.Core/lib/net45/Microsoft.Azure.KeyVault.Core.dll"

open System
open System.IO
//open Microsoft.Azure
//open Microsoft.WindowsAzure // Namespace for CloudConfigurationManager
open Microsoft.WindowsAzure.Storage // Namespace for CloudStorageAccount
open Microsoft.WindowsAzure.Storage.Blob // Namespace for Blob storage types

let shopId = "shop001"
let storageConnString = @"DefaultEndpointsProtocol=https;AccountName=powershop;AccountKey=80WT1uoDd9dALEwpee021KbWSdgR0zYWEGeqPHm/hQhgul5fjDKXxHb0LaTXp+ofkJ6YnzxFTWkWOawg5Uct5w==" 

// Parse the connection string and return a reference to the storage account.
let storageAccount = CloudStorageAccount.Parse(storageConnString)
let blobClient = storageAccount.CreateCloudBlobClient()

let container = blobClient.GetContainerReference(shopId)

// container.CreateIfNotExists()
 
let blockBlob = container.GetBlockBlobReference("products/images/bike.jpg")

let sourceImagePath = "/users/simonlomax/pictures/Bike.Jpg"
blockBlob.UploadFromFile sourceImagePath
printfn "Bike URI: %A" blockBlob.Uri


open System.Text.RegularExpressions
let (|FirstRegexGroup|_|) pattern input =
   let m = Regex.Match(input,pattern) 
   if (m.Success) then Some m.Groups.[1].Value else None  

let testRegex str = 
    match str with
    | FirstRegexGroup "http://(.*?)/(.*)" host -> 
           printfn "The value is a url and the host is %s" host
    | FirstRegexGroup ".*?@(.*)" host -> 
           printfn "The value is an email and the host is %s" host
    | _ -> printfn "The value '%s' is something else" str
   
// test
testRegex "http://google.com/test"
testRegex "alice@hotmail.com"


open System.Text.RegularExpressions
let (|FileExtension|_|) pattern input =
   let m = Regex.Match(input,pattern) 
   if (m.Success) then Some m.Groups.[1].Value else None  

let testFileExtension str =
    match str with
    | FileExtension @"(\.[^\\]+)$" ext -> printfn "File extension is : %s" ext
    | _ -> printfn "File extension not found" 

testFileExtension "an-image.jpg"
testFileExtension "an-imagejpg"
testFileExtension "an-image\jpg"
testFileExtension "an-image/jpg"

open System.IO
printfn "Filename w/out extension: %s" <| Path.GetFileNameWithoutExtension(@"xxx\an-image.jpg")
printfn "Extension: %s" <| Path.GetExtension("an-image.jpg")