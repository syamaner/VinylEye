using VinylEye.Core.Model;

namespace VinylEye.Core.Feature.Matchers;

public class FlannFeatureMatcherFactory: IFlannFeatureMatcherFactory
{
    private readonly Dictionary<string, FlannFeatureMatcher>
        _flannBasedMatchersCache = new();

    public FlannFeatureMatcher Create(string lookupKey, PhotoFeatures[] photoFeatures)
    {
        if (_flannBasedMatchersCache.TryGetValue(lookupKey, out var value))
            return value;
         
        var matcher = new FlannFeatureMatcher(photoFeatures);
        _flannBasedMatchersCache.Add(lookupKey, matcher);
        return matcher;
    }
}