using VinylEye.Core.Model;

namespace VinylEye.Core.Feature.Matchers;

public interface IFlannFeatureMatcherFactory
{
    FlannFeatureMatcher Create(string key, PhotoFeatures[] photoFeatures);
}