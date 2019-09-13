---
page_type: sample
languages:
- javascript
products:
- azure
description: "This sample demonstrates how to interact with the Azure Data Lake Storage Gen1 service using the .NET SDK."
urlFragment: data-lake-store-adls-dot-net-get-started
---

# Azure Data Lake Storage Gen1 .Net: Getting Started

This sample demonstrates how to interact with the Azure Data Lake Storage Gen1 service using the .NET SDK. The sample walks through following main steps:
- Acquire an Azure ActiveDirectory OAuth token (ServiceClientCredential)
- Create a Data Lake Storage Gen1 client object using the account path and OAuth token instance.
- Use the methods on the client object to interact with the store.
- Get a write stream and write a file on store.
- Get a read stream and read the file from store.
- Rename file.
- Enumerate directories and delete directories recursively.
