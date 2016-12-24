#r "../../packages/WindowsAzure.Storage/lib/net45/Microsoft.WindowsAzure.Storage.dll"
#r "../../packages/Microsoft.Azure.KeyVault.Core/lib/net45/Microsoft.Azure.KeyVault.Core.dll"

//#r "packages/WindowsAzure.Storage.8.0.0/lib/net45/Microsoft.WindowsAzure.Storage.dll"
//#r "packages/Microsoft.WindowsAzure.ConfigurationManager.3.2.3/lib/net40/Microsoft.WindowsAzure.Configuration.dll"

open System
open System.IO
//open Microsoft.Azure
//open Microsoft.WindowsAzure // Namespace for CloudConfigurationManager
open Microsoft.WindowsAzure.Storage // Namespace for CloudStorageAccount
open Microsoft.WindowsAzure.Storage.Blob // Namespace for Blob storage types

// Parse the connection string and return a reference to the storage account.
//let storageConnString = "DefaultEndpointsProtocol=https;AccountName=powershop;AccountKey=3yOHdp1t0dGNi91+YAsIYd3Q6D9ZGQCd/xqpq0SJ4TJLSS2TnnPaFMBYgMr/KKmLwm5kE9rLVnA8H/jrVxf9Fg==";

//let storageConnString = CloudConfigurationManager.GetSetting("StorageConnectionString")

let storageConnString = @"DefaultEndpointsProtocol=https;AccountName=powershop;AccountKey=3yOHdp1t0dGNi91+YAsIYd3Q6D9ZGQCd/xqpq0SJ4TJLSS2TnnPaFMBYgMr/KKmLwm5kE9rLVnA8H/jrVxf9Fg==" 

// Parse the connection string and return a reference to the storage account.
let storageAccount = CloudStorageAccount.Parse(storageConnString)

let blobClient = storageAccount.CreateCloudBlobClient()


// Retrieve a reference to a container.
let container = blobClient.GetContainerReference("shop001")

// Create the container if it doesn't already exist.
container.CreateIfNotExists()

let permissions = BlobContainerPermissions(PublicAccess=BlobContainerPublicAccessType.Blob)
container.SetPermissions(permissions)

// Retrieve reference to a blob named "myblob.txt".
let blockBlob = container.GetBlockBlobReference("products/images/uploaded-file.txt")

// Create or overwrite the "myblob.txt" blob with contents from the local file.
// Create a dummy file to upload
let currDir = "/Users/simonlomax/Documents/Development/fsharp-projects/PowershopAzureImages/src/"
let localFile = currDir + "myfile.txt"
File.WriteAllText(localFile, "some data")
blockBlob.UploadFromFile(localFile) 


// Loop over items within the container and output the length and URI.
for item in container.ListBlobs(null, false) do
    match item with 
    | :? CloudBlockBlob as blob -> 
        printfn "Block blob of length %d: %O" blob.Properties.Length blob.Uri

    | :? CloudPageBlob as pageBlob ->
        printfn "Page blob of length %d: %O" pageBlob.Properties.Length pageBlob.Uri

    | :? CloudBlobDirectory as directory ->
        printfn "Directory: %O" directory.Uri

    | _ ->
        printfn "Unknown blob type: %O" (item.GetType())