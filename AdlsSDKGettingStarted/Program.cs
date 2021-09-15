using Azure;
using Azure.Identity;
using Azure.Storage.Files.DataLake;
using Azure.Storage.Files.DataLake.Models;
using System;
using System.IO;
using System.Text;

namespace AdlsSDKGettingStarted
{
    public class Program
    {
        private static string applicationId = "FILL-IN-HERE";     // Also called client id
        private static string clientSecret = "FILL-IN-HERE";
        private static string tenantId = "FILL-IN-HERE";
        private static string serviceUri = "FILL-IN-HERE";        // full account FQDN, not just the account name - it should look like https://{ACCOUNTNAME}.dfs.core.windows.net/

        public static void Main(string[] args)
        {
            // Create Client Secret Credential
            var creds = new ClientSecretCredential(tenantId, applicationId, clientSecret);

            // Create data lake file service client object
            DataLakeServiceClient serviceClient = new DataLakeServiceClient(new Uri(serviceUri), creds);
            var name = "sample-filesystem" + Guid.NewGuid().ToString("n").Substring(0, 8);
            // Create data lake file system client object
            DataLakeFileSystemClient filesystemclient = serviceClient.GetFileSystemClient(name);
            
            filesystemclient.CreateIfNotExists();

            try
            {
                long length;
                string fileName = "/Test/testFilename1.txt";
                DataLakeFileClient file = filesystemclient.GetFileClient(fileName);

                // Upload a file - automatically creates any parent directories that don't exist
                length = BinaryData.FromString("This is test data to write.\r\nThis is the second line.\r\n").ToStream().Length;

                file.Upload(BinaryData.FromString("This is test data to write.\r\nThis is the second line.\r\n").ToStream(),true);

                file.Append(BinaryData.FromString("This is the added line.\r\n").ToStream(), length);
                file.Flush(length + BinaryData.FromString("This is the added line.\r\n").ToStream().Length);
                //Read file contents
                Response<FileDownloadInfo> fileContents = file.Read();
                
                Console.WriteLine(BinaryData.FromStream(fileContents.Value.Content).ToString());

                // Get the properties of the file
                PathProperties pathProperties = file.GetProperties();
                PrintDirectoryEntry(pathProperties);

                // Rename a file
                string destFilePath = "/Test/testRenameDest3.txt";
                file.Rename(destFilePath);
                Console.WriteLine("The file URI is "+ file.Uri);

                // Enumerate directory
                foreach (var pathItem in filesystemclient.GetPaths("/Test"))
                {
                    PrintDirectoryEntry(pathItem);
                }

                // Delete a directory and all it's subdirectories and files
                filesystemclient.DeleteDirectory("/Test");

            }
            finally
            {
                filesystemclient.Delete();
            }

            Console.WriteLine("Done. Press ENTER to continue ...");
            Console.ReadLine();
        }
        
        private static void PrintDirectoryEntry(PathProperties pathProperties)
        {
            Console.WriteLine($"ExpiresOn Time: {pathProperties.ExpiresOn}");
            Console.WriteLine($"ContentType: {pathProperties.ContentType}");
            Console.WriteLine($"Metadata: {pathProperties.Metadata}");
            Console.WriteLine($"Created Time: {pathProperties.CreatedOn}");
            Console.WriteLine($"Length: {pathProperties.ContentLength}");
            Console.WriteLine($"Modified Time: {pathProperties.LastModified}");
            Console.WriteLine();
        }
        private static void PrintDirectoryEntry(PathItem pathItem)
        {
            Console.WriteLine($"Name: {pathItem.Name}");
            Console.WriteLine($"Length: {pathItem.ContentLength}");
            Console.WriteLine($"User: {pathItem.Owner}");
            Console.WriteLine($"Group: {pathItem.Group}");
            Console.WriteLine($"Permission: {pathItem.Permissions}");
            Console.WriteLine($"Modified Time: {pathItem.LastModified}");
            Console.WriteLine();
        }
    }
}