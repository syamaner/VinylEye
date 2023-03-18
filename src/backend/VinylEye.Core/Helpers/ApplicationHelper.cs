using Microsoft.Extensions.Options;
using VinylEye.Core.Model;
using VinylEye.Core.Options;

namespace VinylEye.Core.Helpers;

public class ApplicationHelper : IApplicationHelper
{
    public ApplicationOptions ApplicationOptions { get; set; }

    private readonly IDictionary<ApplicationDirectoryTypes, string> _folderMap;
    
    public ApplicationHelper(IOptions<ApplicationOptions> applicationOptions)
    {
        ApplicationOptions = applicationOptions.Value;

        _folderMap = new Dictionary<ApplicationDirectoryTypes, string>()
        {
            {ApplicationDirectoryTypes.ExportedImageDirectory , ApplicationOptions.ExportedImageDirectoryName},
            {ApplicationDirectoryTypes.DatabaseDirectory , ApplicationOptions.DatabaseDirectoryName},
            {ApplicationDirectoryTypes.ImageDownloadDirectory , ApplicationOptions.ImageDownloadDirectoryName},
            {ApplicationDirectoryTypes.IndexDirectory , ApplicationOptions.IndexDirectoryName},
            {ApplicationDirectoryTypes.TestDirectory , ApplicationOptions.TestDirectoryName}
        };
    }

    public string GetApplicationDirectory(ApplicationDirectoryTypes applicationDirectoryType)
    {
        var path = Path.Combine(ApplicationOptions.ApplicationDataDirectory, _folderMap[applicationDirectoryType]);

        path.CreateDirectoryIfNotExits();

        return path;
    }
}