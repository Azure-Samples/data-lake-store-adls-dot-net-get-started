---
page_type: sample
languages:
- csharp
products:
- azure
description: "This sample demonstrates how to interact with the Azure Data Lake Storage Gen1 service using the .NET SDK."
urlFragment: get-started-net-azure-data-lake
---

# Azure Data Lake Storage Gen1 .NET: Getting Started

This sample demonstrates how to interact with the Azure Data Lake Storage Gen1 service using the .NET SDK. The sample walks through following main steps:
- Acquire an Azure ActiveDirectory OAuth token (ServiceClientCredential)
- Create a Data Lake Storage Gen1 client object using the account path and OAuth token instance.
- Use the methods on the client object to interact with the store.
- Get a write stream and write a file on store.
- Get a read stream and read the file from store.
- Rename file.
- Enumerate directories and delete directories recursively.
