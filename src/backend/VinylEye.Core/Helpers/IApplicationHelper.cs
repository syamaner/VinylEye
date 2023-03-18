using VinylEye.Core.Model;

namespace VinylEye.Core.Helpers;

public interface IApplicationHelper
{
    public string GetApplicationDirectory(ApplicationDirectoryTypes applicationDirectoryType);
}