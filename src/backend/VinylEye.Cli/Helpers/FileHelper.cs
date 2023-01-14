namespace VinylEye.Cli.Helpers;

public static class FileHelper
{
    public static void EnsureFilesExistsInOutputDirectory(string outputDirectory, params string[] filesToCopy)
    {
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);
        
        foreach (var file in filesToCopy)
        {
            var fileInfo = new FileInfo(file);
            var destinationPath = Path.Combine(outputDirectory, fileInfo.Name);
            if(!File.Exists(destinationPath))
                fileInfo.CopyTo(destinationPath);
        }
    }
}