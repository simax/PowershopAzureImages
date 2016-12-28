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
