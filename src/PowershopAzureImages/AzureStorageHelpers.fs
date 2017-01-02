namespace PowershopAzureImages

module AzureStorageHelpers = 

    open System
    open System.IO
    //open Microsoft.Azure
    //open Microsoft.WindowsAzure // Namespace for CloudConfigurationManager
    open Microsoft.WindowsAzure.Storage // Namespace for CloudStorageAccount
    open Microsoft.WindowsAzure.Storage.Blob // Namespace for Blob storage types

    let GetShopContainer shopId = 
        let storageConnString = @"DefaultEndpointsProtocol=https;AccountName=powershop;AccountKey=80WT1uoDd9dALEwpee021KbWSdgR0zYWEGeqPHm/hQhgul5fjDKXxHb0LaTXp+ofkJ6YnzxFTWkWOawg5Uct5w==" 
        let storageAccount = CloudStorageAccount.Parse(storageConnString)
        let blobClient = storageAccount.CreateCloudBlobClient()
        let container  = blobClient.GetContainerReference(shopId)
        container
    
    let uploadFile (shopContainer:CloudBlobContainer) sourcePath destinationPath = 
        shopContainer.CreateIfNotExists() |> ignore
        let blockBlob = shopContainer.GetBlockBlobReference(destinationPath) 
        async {
            do! blockBlob.UploadFromFileAsync sourcePath
            return blockBlob.Uri.ToString()
        } |> Async.RunSynchronously
        

    let deleteFile (shopContainer:CloudBlobContainer) imageUrl = 
        let blockBlob = shopContainer.GetBlockBlobReference(imageUrl) 
        async {
            let exists = blockBlob.Exists()
            do! blockBlob.DeleteIfExistsAsync()
            return exists
        } |> Async.RunSynchronously
           
