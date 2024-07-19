using System.IO.Compression;

namespace ZipExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3 || string.IsNullOrWhiteSpace(args[0]) || string.IsNullOrWhiteSpace(args[1]))
            {
                Console.WriteLine("Usage: ZipExtractor <source zip path> <target extraction path>");
                throw new ArgumentException("Usage: ZipExtractor <source zip path> <target extraction path>");
            }


            string sourceZipPath = args[0];
            string targetExtractionPath = args[1];
            int operationPerformed = Convert.ToInt32(args[2]);           

            if (!IsValidPath(targetExtractionPath))
            {
                Console.WriteLine($"Error: The target extraction path '{targetExtractionPath}' is not a valid path.");
                throw new DirectoryNotFoundException($"Error: The target extraction path '{targetExtractionPath}' is not a valid path.");
            }
            try
            {
                if (operationPerformed == (int)OperationPerformed.ZipExtractor)
                {
                    ZipExtractor(sourceZipPath, targetExtractionPath);
                }
                else if (operationPerformed == (int)OperationPerformed.CopyDirectory)
                {
                    CopyDirectory(AddLongPathPrefix(sourceZipPath), AddLongPathPrefix(targetExtractionPath));
                    Console.WriteLine("Data copied successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while performing the operation: {ex.Message}");
                throw new Exception($"An error occurred while performing the operation: {ex.Message}");
            }

        }

        private static void ZipExtractor(string sourceZipPath, string targetExtractionPath)
        {
            if (!File.Exists(sourceZipPath))
            {
                Console.WriteLine($"Error: The source zip file '{sourceZipPath}' does not exist.");
                throw new FileNotFoundException($"Error: The source zip file '{sourceZipPath}' does not exist.");
            }

            try
            {
                if (!Directory.Exists(targetExtractionPath))
                {
                    Directory.CreateDirectory(targetExtractionPath);
                    Console.WriteLine($"The directory '{targetExtractionPath}' did not exist and was created.");
                }

                ZipFile.ExtractToDirectory(sourceZipPath, targetExtractionPath);
                Console.WriteLine($"Successfully extracted '{sourceZipPath}' to '{targetExtractionPath}'");
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine($"Error: {ex.Message} ");
                throw;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"I/O Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }

        public static void CopyDirectory(string sourceDir, string destinationDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDir);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destinationDir);

            try
            {
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(destinationDir, file.Name);

                    file.CopyTo(tempPath, true);
                }

                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destinationDir, subdir.Name);

                    CopyDirectory(subdir.FullName, tempPath);
                }
            }
            catch (InvalidDataException ex)
            {
                Console.WriteLine($"Error: {ex.Message} ");
                throw;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"I/O Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }

        private static string AddLongPathPrefix(string path)
        {
            if (path.StartsWith(@"\\?\"))
            {
                return path;
            }
            return @"\\?\" + path.Replace("/", @"\");
        }

        static bool IsValidPath(string path)
        {
            try
            {
                // Check for invalid characters
                char[] invalidChars = Path.GetInvalidPathChars();
                if (path.Any(c => invalidChars.Contains(c)))
                {
                    return false;
                }

                // Attempt to get the full path to check for other potential issues
                Path.GetFullPath(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
