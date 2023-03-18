using OpenCvSharp;
using OpenCvSharp.Features2D;
using VinylEye.Core.Model;

namespace VinylEye.Core.FeatureExtractors;

public class SiftFeatureExtractor : FeatureExtractorBase
{

    public SiftFeatureExtractor() : base(FeatureExtractorTypes.Sift)
    {
    }

    public override (Mat, KeyPoint[]) GetDescriptors(Mat image)
    {
        Mat descriptors = new();
        using var sift = SIFT.Create(nFeatures:1000);//nFeatures:500
        sift.DetectAndCompute(image, null, out var keyPoints, descriptors);
       // _logger.LogInformation("n key points {kp} w: {w} h: {h}",keyPoints.Length, descriptors.Width, descriptors.Height);
        return (descriptors, keyPoints);
    }
}