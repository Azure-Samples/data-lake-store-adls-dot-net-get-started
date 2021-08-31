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
        private static string serviceUri = "FILL-IN-HERE";        // full account FQDN, not just the account name like https://{ACCOUNTNAME}.dfs.core.windows.net/

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
                using (var stream = new MemoryStream())
                {
                    byte[] textByteArray = Encoding.UTF8.GetBytes("This is test data to write.\r\n");
                    stream.Write(textByteArray, 0, textByteArray.Length);

                    textByteArray = Encoding.UTF8.GetBytes("This is the second line.\r\n");
                    stream.Write(textByteArray, 0, textByteArray.Length);
                    length = stream.Length;
                    stream.Seek(0, SeekOrigin.Begin);
                    file.Upload(stream, true);
                }

                // Append to existing file
                using (var stream = new MemoryStream())
                {
                    byte[] textByteArray = Encoding.UTF8.GetBytes("This is the added line.\r\n");
                    stream.Write(textByteArray, 0, textByteArray.Length);
                    stream.Seek(0, SeekOrigin.Begin);
                    file.Append(stream, length);
                    file.Flush(length + stream.Length);
                }

                //Read file contents
                Response<FileDownloadInfo> fileContents = file.Read();
                using (var readStream = new StreamReader(fileContents.Value.Content))
                {
                    string line;
                    while ((line = readStream.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }

                // Get the properties of the file
                PathProperties pathProperties = file.GetProperties();
                PrintDirectoryEntry(pathProperties);

                // Rename a file
                string destFilePath = "/Test/testRenameDest3.txt";
                file.Rename(destFilePath);

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