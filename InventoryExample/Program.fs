
open Migrations.Model

module Gallery =        
    let Folder = "GalleryFolder"
    let File = "Gallery.File"
  
module Inventory =        
    let Product = "Inventory.Product"

module Platforms =    
    module Nventstore =        
        let Product = "Platforms.Nventstore.NventstoreProduct"

module Support =        
    let Audit = "Platforms.Nventstore.NventstoreProduct"


open Migrations


let migrations = [

    migration "2010_05_15_NventstoreProduct_DefaultImage" [
        alterEntity Platforms.Nventstore.Product [
            reference "DefaultImage" Gallery.File
            references "Images" Gallery.File
        ]

        script [
            "PRINT N'Migrating NventstoreProductToFile data...';

            INSERT INTO [Platforms.Nventstore].[NventstoreProductToFile]
                   ([NventstoreProductID]
                   ,[NventstoreImageID])           
	        SELECT     
		        a.NventstoreProductID, f.FileID
	        FROM         
		        [Platforms.Nventstore].NventstoreProductImageAssignee AS a 
		        INNER JOIN Gallery.[File] AS f ON a.ImageKey = f.UniqueKey"

            "PRINT N'Creating [DefaultImageID] data...';		

            UPDATE [Platforms.Nventstore].[NventstoreProduct]
            SET [DefaultImageID] = ( SELECT TOP 1 f.[NventstoreImageID]  
							FROM [Platforms.Nventstore].NventstoreProductToFile as f
							WHERE f.[NventstoreProductID] = [Platforms.Nventstore].[NventstoreProduct].[NventstoreProductID] )"
        ]
    ]

    migration "2010_05_16_Audit" [
        createRecord Support.Audit [
            primmaryKey
            xml "Data"
            support
            context
        ]
    ]

    migration "2010_05_17_Platforms_Nventstore_NventstoreProduct_IsAssigned" [
        alterEntity Platforms.Nventstore.Product [ 
            drop "CanPublish" 
            drop "Assigned"

            boolean "CanPublish"
            boolean "IsAssigned"
            boolean "IsPublishEnabled"
        ]

        script [
            "UPDATE [Platforms.Nventstore].NventstoreProduct SET IsAssigned = 1"
            "UPDATE [Platforms.Nventstore].NventstoreProduct SET IsPublishEnabled = ISNULL(PublishEnabled, 1)"
            "UPDATE [Platforms.Nventstore].NventstoreProduct SET CanPublish = IsPublishEnabled"
        ]

        alterEntity Platforms.Nventstore.Product [ 
            drop "PublishEnabled"
        ]
    ]
     
    migration "2010_05_18_Inventory_Product_IsDeleted" [
        alterEntity Inventory.Product [             
            boolean "IsDeleted"            
        ]
    ]
]