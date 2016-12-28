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

        // Parse the connection string and return a reference to the storage account.
        let storageAccount = CloudStorageAccount.Parse(storageConnString)
        let blobClient = storageAccount.CreateCloudBlobClient()

        blobClient.GetContainerReference(shopId)

    // container.CreateIfNotExists()
    
    let uploadFile (shopContainer:CloudBlobContainer) sourcePath destinationPath = 
        let blockBlob = shopContainer.GetBlockBlobReference(destinationPath) //"products/images/bike.jpg"

        // let sourceImagePath = "/users/simonlomax/pictures/Bike.Jpg"
        blockBlob.UploadFromFile sourcePath
        blockBlob.Uri.ToString()
