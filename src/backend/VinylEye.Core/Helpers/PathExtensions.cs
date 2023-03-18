namespace VinylEye.Core.Helpers;

public static class PathExtensions
{
    public static void CreateDirectoryIfNotExits(this string filePath)
    {
        var dir = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir!);
    }

    public static void EnsureFilesExistsInOutputDirectory(string outputDirectory, params string[] filesToCopy)
    {
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        foreach (var file in filesToCopy)
        {
            var fileInfo = new FileInfo(file);
            var destinationPath = Path.Combine(outputDirectory, fileInfo.Name);
            if (!File.Exists(destinationPath))
                fileInfo.CopyTo(destinationPath);
        }
    }
}