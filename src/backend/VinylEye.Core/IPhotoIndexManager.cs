using VinylEye.Core.Model;

namespace VinylEye.Core;

public interface IPhotoIndexManager
{
    PhotoFeatures[] LoadFeatures(string indexFileName);
    Task SaveFeatures(PhotoFeatures[] photos);
    Task<PhotoFeatures[]> IndexAll(string indexDirectory, string exportedImagesDirectory);
}