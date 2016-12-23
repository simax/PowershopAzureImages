#r "../../packages/WindowsAzure.Storage/lib/net45/Microsoft.WindowsAzure.Storage.dll"


open System
open System.IO
open Microsoft.WindowsAzure // Namespace for CloudConfigurationManager
open Microsoft.WindowsAzure.Storage // Namespace for CloudStorageAccount
open Microsoft.WindowsAzure.Storage.Blob // Namespace for Blob storage types

open Microsoft.WindowsAzure.Storage

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
let blockBlob = container.GetBlockBlobReference("myblob.txt")

// Create or overwrite the "myblob.txt" blob with contents from the local file.
// Create a dummy file to upload
let localFile = __SOURCE_DIRECTORY__ + "/myfile.txt"
File.WriteAllText(localFile, "some data")
blockBlob.UploadFromFile(localFile, FileMode.CreateNew) 

