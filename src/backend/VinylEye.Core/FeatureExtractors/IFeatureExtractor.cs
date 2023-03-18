using OpenCvSharp;
using VinylEye.Core.Model;

namespace VinylEye.Core.FeatureExtractors;

public interface IFeatureExtractor
{
    (Mat, KeyPoint[]) GetDescriptors(Mat image);
    bool SupportsFeatureType(FeatureExtractorTypes featureExtractorType);
}