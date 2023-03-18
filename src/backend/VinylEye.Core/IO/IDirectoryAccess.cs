namespace VinylEye.Core.IO;

public interface IDirectoryAccess
{
    IEnumerable<string> ListFiles(string baseDirectory, params string[] fileExtensionsToInclude);

    Dictionary<string, int> GetFileTypes(string directory, params string[] fileExtensionsToExclude);

}