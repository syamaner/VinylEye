using VinylEye.Core.Model;

namespace VinylEye.Core;

public interface IPhotoSearcher
{
    ReleaseInformation? QueryIndex(ReleaseQuery releaseQuery);
    void EnsureIndex();
}