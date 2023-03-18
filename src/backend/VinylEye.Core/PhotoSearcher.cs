using System.Diagnostics;
using Microsoft.Extensions.Options;
using VinylEye.Core.Feature.Matchers;
using VinylEye.Core.IO;
using VinylEye.Core.Model;
using VinylEye.Core.Options;

namespace VinylEye.Core;

public class PhotoSearcher : IPhotoSearcher
{
    private readonly IFlannFeatureMatcherFactory _featureMatcherFactory;
    private readonly IPhotoIndexManager _indexManager;
    private FlannFeatureMatcher? _featureMatcher;

    private readonly IPhotoSerialiser _photoProcessor;
    private readonly ApplicationOptions _applicationOptions;

    public PhotoSearcher(IFlannFeatureMatcherFactory featureMatcherFactory,  IPhotoIndexManager indexManager, IPhotoSerialiser photoProcessor,
        IOptions<ApplicationOptions> applicationOptions)
    {
        _featureMatcherFactory =
            featureMatcherFactory ?? throw new ArgumentNullException(nameof(featureMatcherFactory));

        _indexManager = indexManager;
        _photoProcessor = photoProcessor;
        _applicationOptions = applicationOptions.Value;
    }

    public void EnsureIndex()
    {
        if (_featureMatcher == null)
            GetFeatureMatcher();
    }

    public ReleaseInformation? QueryIndex(ReleaseQuery releaseQuery)
    {
        using var image = _photoProcessor.PopulatePhotoInformation(releaseQuery.FilePath);
        
        var matchResults = _featureMatcher!.KnnMatch(image);

        var sorted = matchResults.OrderByDescending(x => x.Value).ToList();

        List<string> distinctPathList = sorted.Select(x => x.Key.PhotoInformation!.ImagePath).Distinct().ToList()!;

        var distinctFileNames = distinctPathList.Select(x => new FileInfo(x).Name).ToList();

        if (distinctFileNames.Any() == false)
            return null;

        var covers = GetReleaseInfo(distinctFileNames);

        var cover = covers[distinctFileNames.First()];

        return cover;

    }

    private void GetFeatureMatcher()
    {
        var photos = _indexManager.LoadFeatures(_applicationOptions.IndexFileName);
        _featureMatcher = _featureMatcherFactory.Create(_applicationOptions.IndexFileName, photos);
    }

    private Dictionary<string, ReleaseInformation?> GetReleaseInfo(IEnumerable<string> fileNames)
    {
        string basePath = Path.Combine(AppContext.BaseDirectory, _applicationOptions.ApplicationDataDirectory);
        var coverArtList = new List<PhotoInfo?>();
        foreach (var filename in fileNames)
        {
            var file = Directory.EnumerateFiles(basePath, filename,SearchOption.AllDirectories).SingleOrDefault();
            var art =  _photoProcessor.PopulatePhotoInformation(file!);

            Debug.Assert(art != null, nameof(art) + " != null");
            coverArtList.Add(art.PhotoInformation);
        }

        return coverArtList.ToDictionary(y => new FileInfo(y?.ImagePath!).Name, y => y?.ReleaseMetadata!);
    }

}