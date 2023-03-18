using Microsoft.Extensions.Options;
using ProtoBuf;
using VinylEye.Core.Helpers;
using VinylEye.Core.IO;
using VinylEye.Core.Model;
using VinylEye.Core.Options;

namespace VinylEye.Core;

public class PhotoIndexManager : IPhotoIndexManager
{
    private readonly IDirectoryAccess _directoryAccess;
    private readonly IPhotoSerialiser _imageSerializer;
    private readonly ApplicationOptions _applicationOptions;
    private readonly string _indexDirectory;
    private readonly IPhotoSerialiser _photoProcessor;
    private readonly IPhotoSerialiser _photoSerialiser;

    public PhotoIndexManager(IApplicationHelper applicationHelper, IPhotoSerialiser imageSerializer, IOptions<ApplicationOptions> options, IDirectoryAccess directoryAccess, IPhotoSerialiser photoProcessor, IPhotoSerialiser photoSerialiser)
    {
        _imageSerializer = imageSerializer;
        _directoryAccess = directoryAccess;
        _photoProcessor = photoProcessor;
        _photoSerialiser = photoSerialiser;
        _applicationOptions = options.Value;
        _indexDirectory = applicationHelper.GetApplicationDirectory(ApplicationDirectoryTypes.IndexDirectory);
    }

    public PhotoFeatures[] LoadFeatures(string indexFileName = "index_sift_800_400.bin")//"index_sift_260_140.bin"
    {
        var indexFilePath = Path.Combine(_indexDirectory, indexFileName);

        using var file = File.OpenRead(indexFilePath);
        var photos = Serializer.Deserialize<PhotoFeatures[]>(file);
        
        DeserialiseDescriptors(photos);

        return photos;
    }

    public async Task SaveFeatures(PhotoFeatures[] photoFeatures)
    {
        var indexFileName = Path.Combine(_indexDirectory
            , $"index_sift_{_applicationOptions.MaxProcessingImageWidth}_{_applicationOptions.MaxProcessingImageHeight}.bin");
     
        if (File.Exists(indexFileName))
            File.Delete(indexFileName);

        await using var file = File.Create(indexFileName);
        Serializer.Serialize(file, photoFeatures);
    }

    public virtual async Task<PhotoFeatures[]> IndexAll(string indexDirectory, string exportedImagesDirectory)
    {

        var files = await GetImageFilesToIndex(exportedImagesDirectory, indexDirectory);

        var images = new PhotoFeatures[files.Count];

        var options = new ParallelOptions { MaxDegreeOfParallelism = _applicationOptions.MaxCores };
        Parallel.ForEach(files, options, (coverArtFile, _, index) =>
        {
            var photoInfo = _photoProcessor.PopulatePhotoInformation(coverArtFile);
            photoInfo.DescriptorsToSave = _photoSerialiser.SerialiseDescriptors(photoInfo.Descriptors!);
            images[index] = photoInfo;
        });


        await SaveIndexFileAsync(indexDirectory, images);

        return images;
    }

    private async Task<List<string>> GetImageFilesToIndex(string exportedImagesDirectory, string indexDirectory)
    {
        var ext = _directoryAccess.GetFileTypes(exportedImagesDirectory, "bin", "json", "pdf");

        var files = _directoryAccess.ListFiles(exportedImagesDirectory, ext.Keys.ToArray()).ToList();
        await SaveFileNamesUsedInIndexFileAsync(indexDirectory, files);
        return files;
    }

    private async Task SaveIndexFileAsync(string indexDirectory, PhotoFeatures[] photos)
    {
        var indexFileName = Path.Combine(indexDirectory
            , $"index_sift_{_applicationOptions.MaxProcessingImageWidth}_{_applicationOptions.MaxProcessingImageHeight}.bin");
        indexDirectory.CreateDirectoryIfNotExits();
        if (File.Exists(indexFileName))
            File.Delete(indexFileName);

        await using var file = File.Create(indexFileName);
        Serializer.Serialize(file, photos);
    }


    private static async Task SaveFileNamesUsedInIndexFileAsync(string indexDirectory, IEnumerable<string> files)
    {
        var fileName = Path.Combine(indexDirectory, "images.json");

        if (File.Exists(fileName))
            File.Delete(fileName);

        await File.WriteAllLinesAsync(fileName, files);
    }

    private void DeserialiseDescriptors(PhotoFeatures[] photos)
    {
        var options = new ParallelOptions { MaxDegreeOfParallelism = _applicationOptions.MaxCores };
        Parallel.ForEach(photos, options,
            (photo, _, index) =>
            {
                photos[index].Descriptors = _imageSerializer.DeSerialiseDescriptors(photo.DescriptorsToSave!);
                photo.DescriptorsToSave = null;
            });
    }

}