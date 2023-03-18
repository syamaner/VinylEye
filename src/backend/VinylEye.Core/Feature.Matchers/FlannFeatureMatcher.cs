using OpenCvSharp;
using OpenCvSharp.Flann;
using VinylEye.Core.Model;

namespace VinylEye.Core.Feature.Matchers;

public class FlannFeatureMatcher : IDisposable
{
    private readonly PhotoFeatures[] _indexPhotos;
    private readonly FlannBasedMatcher _matcher;
    private const int DistanceThreshold = 220;

    public FlannFeatureMatcher(PhotoFeatures[] indexPhotos)
    {
        _indexPhotos = indexPhotos;
        var kdTreeIndexParams = new KDTreeIndexParams(5);
        var searchParams = new SearchParams(50);

        _matcher = new FlannBasedMatcher(kdTreeIndexParams, searchParams);
        _matcher.Add(indexPhotos.Select(x => x.Descriptors).ToList()!);
        _matcher.Train();
    }


    public Dictionary<PhotoFeatures, int> KnnMatch(PhotoFeatures toMatch, double kRatioThresh = 0.75)
    {
        var matches = _matcher.KnnMatch(toMatch.Descriptors!, 2);

        var goodMatches =
            (from match in matches where match[0].Distance < kRatioThresh * match[1].Distance select match[0]).ToList();

        var groupedMatches = goodMatches.GroupBy(x => x.ImgIdx, x => x)
            .ToDictionary(x => x.Key, x => x.ToList());

        var indexMatchCount = new Dictionary<PhotoFeatures, int>();

        foreach (var kvp in groupedMatches)
        {
            var count = kvp.Value.Count(x => x.Distance < DistanceThreshold);
            if (count > 2)
                indexMatchCount[_indexPhotos[kvp.Key]] = count;
        }

        return indexMatchCount;
    }

    public void Dispose() => _matcher.Dispose();
}