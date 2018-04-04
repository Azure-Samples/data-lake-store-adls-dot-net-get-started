using System;
using System.IO;
using System.Text;
using Microsoft.Azure.DataLake.Store;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;

namespace AdlsSDKGettingStarted
{
    public class Program
    {
        private static string applicationId = "FILL-IN-HERE";     // Also called client id
        private static string clientSecret = "FILL-IN-HERE";
        private static string tenantId = "FILL-IN-HERE";
        private static string adlsAccountFQDN = "FILL-IN-HERE";   // full account FQDN, not just the account name like example.azuredatalakestore.net

        public static void Main(string[] args)
        {
            // Obtain AAD token
            var creds = new ClientCredential(applicationId, clientSecret);
            var clientCreds = ApplicationTokenProvider.LoginSilentAsync(tenantId, creds).GetAwaiter().GetResult();

            // Create ADLS client object
            AdlsClient client = AdlsClient.CreateClient(adlsAccountFQDN, clientCreds);

            try
            {
                string fileName = "/Test/testFilename1.txt";

                // Create a file - automatically creates any parent directories that don't exist
                // The AdlsOuputStream preserves record boundaries - it does not break records while writing to the store
                using (var stream = client.CreateFile(fileName, IfExists.Overwrite))
                {
                    byte[] textByteArray = Encoding.UTF8.GetBytes("This is test data to write.\r\n");
                    stream.Write(textByteArray, 0, textByteArray.Length);

                    textByteArray = Encoding.UTF8.GetBytes("This is the second line.\r\n");
                    stream.Write(textByteArray, 0, textByteArray.Length);
                }

                // Append to existing file
                using (var stream = client.GetAppendStream(fileName))
                {
                    byte[] textByteArray = Encoding.UTF8.GetBytes("This is the added line.\r\n");
                    stream.Write(textByteArray, 0, textByteArray.Length);
                }

                //Read file contents
                using (var readStream = new StreamReader(client.GetReadStream(fileName)))
                {
                    string line;
                    while ((line = readStream.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }

                // Get the properties of the file
                var directoryEntry = client.GetDirectoryEntry(fileName);
                PrintDirectoryEntry(directoryEntry);

                // Rename a file
                string destFilePath = "/Test/testRenameDest3.txt";
                client.Rename(fileName, destFilePath, true);

                // Enumerate directory
                foreach (var entry in client.EnumerateDirectory("/Test"))
                {
                    PrintDirectoryEntry(entry);
                }

                // Delete a directory and all it's subdirectories and files
                client.DeleteRecursive("/Test");

            }
            catch (AdlsException e)
            {
                PrintAdlsException(e);
            }

            Console.WriteLine("Done. Press ENTER to continue ...");
            Console.ReadLine();
        }
        
        private static void PrintDirectoryEntry(DirectoryEntry entry)
        {
            Console.WriteLine($"Name: {entry.Name}");
            Console.WriteLine($"FullName: {entry.FullName}");
            Console.WriteLine($"Length: {entry.Length}");
            Console.WriteLine($"Type: {entry.Type}");
            Console.WriteLine($"User: {entry.User}");
            Console.WriteLine($"Group: {entry.Group}");
            Console.WriteLine($"Permission: {entry.Permission}");
            Console.WriteLine($"Modified Time: {entry.LastModifiedTime}");
            Console.WriteLine($"Last Accessed Time: {entry.LastAccessTime}");
            Console.WriteLine();
        }

        private static void PrintAdlsException(AdlsException exp)
        {
            Console.WriteLine("ADLException");
            Console.WriteLine($"   Http Status: {exp.HttpStatus}");
            Console.WriteLine($"   Http Message: {exp.HttpMessage}");
            Console.WriteLine($"   Remote Exception Name: {exp.RemoteExceptionName}");
            Console.WriteLine($"   Server Trace Id: {exp.TraceId}");
            Console.WriteLine($"   Exception Message: {exp.Message}");
            Console.WriteLine($"   Exception Stack Trace: {exp.StackTrace}");
            Console.WriteLine();
        }
    }
}
