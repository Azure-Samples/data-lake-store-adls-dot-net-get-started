---
services: data-lake-store
platforms: .NET
author: rahuldutta90
---

# Azure Data Lake Store DotNet: Getting Started

This sample demonstrates how to interact with Azure Data Lake Store service using the .NET SDK. The sample walk through following main steps:
- Acquire an Azure ActiveDirectory OAuth token (ServiceClientCredential)
- Create a ADLS client object using the account path and OAuth token instance.
- Use the methods on the client object to interact with the store.
- Get a write stream and write a file on store.
- Get a read stream and read the file from store.
- Rename file.
- Enumerate directories and delete directories recursively.