using OpenCvSharp;
using VinylEye.Core.Model;

namespace VinylEye.Core.FeatureExtractors;

public abstract class FeatureExtractorBase : IFeatureExtractor
{
    private readonly FeatureExtractorTypes _featureExtractorType;

    protected FeatureExtractorBase(FeatureExtractorTypes featureExtractorType) 
    {
        _featureExtractorType = featureExtractorType;
    }

    public abstract (Mat, KeyPoint[]) GetDescriptors(Mat image);

    public bool SupportsFeatureType(FeatureExtractorTypes featureExtractorType)
    {
        return _featureExtractorType == featureExtractorType;
    }
}