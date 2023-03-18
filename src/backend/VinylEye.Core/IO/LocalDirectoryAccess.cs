namespace VinylEye.Core.IO;

public class LocalDirectoryAccess : IDirectoryAccess
{
    public IEnumerable<string> ListFiles(string baseDirectory, params string[] fileExtensionsToInclude)
    {
        return Directory.EnumerateFiles(baseDirectory, "*", SearchOption.AllDirectories).Where(fileName =>
            fileExtensionsToInclude.Any(ext => fileName.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase)));
    }

    public Dictionary<string, int> GetFileTypes(string directory, params string[] fileExtensionsToExclude)
    {
        var fileTypes = new Dictionary<string, int>();

        foreach (var fileName in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
        {
            var fileInfo = new FileInfo(fileName);
            var ext = fileInfo.Extension.ToLower();
            
            if (fileExtensionsToExclude != null && fileExtensionsToExclude.Any(x=>x.Contains(ext,StringComparison.OrdinalIgnoreCase) || ext.Contains(x, StringComparison.OrdinalIgnoreCase)))
                continue;
            
            if (!fileTypes.ContainsKey(ext))
                fileTypes[ext] = 1;
            else
                fileTypes[ext] += 1;
        }

        return fileTypes;
    }
}