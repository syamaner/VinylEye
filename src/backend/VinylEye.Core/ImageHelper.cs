using OpenCvSharp;
using OpenCvSharp.Features2D;

namespace VinylEye.Core;

public static class ImageHelper
{
    /// <summary>
    /// Computes the Homography matrix between two images based on the extracted key points
    ///     and the matched features between source and destination images.
    ///
    /// Reference:
    ///  https://github.com/shimat/opencvsharp/issues/1087
    /// </summary>
    /// <param name="sourceKeyPoints"></param>
    /// <param name="destinationKeyPoints"></param>
    /// <param name="matches"></param>
    /// <param name="minimumNumberOfMatches"></param>
    /// <returns>Homography matrix mapping source image points to the destination image points.</returns>
    public static Mat? CalculateHomographyMatrix(IReadOnlyList<KeyPoint> sourceKeyPoints, 
        IReadOnlyList<KeyPoint> destinationKeyPoints, 
        IReadOnlyCollection<DMatch> matches, 
        int minimumNumberOfMatches=4)
    {
        if (matches.Count <= minimumNumberOfMatches) return null;

        var sourcePoints = new List<Point2f>(matches.Count);
        var destinationPoints = new List<Point2f>(matches.Count);
 
        foreach (var m in matches)
        {
            var sourceKeyPoint = sourceKeyPoints[m.QueryIdx];
            var destinationKeyPoint = destinationKeyPoints[m.TrainIdx];

            sourcePoints.Add(new Point2f(sourceKeyPoint.Pt.X, sourceKeyPoint.Pt.Y));
            destinationPoints.Add(new Point2f(destinationKeyPoint.Pt.X, destinationKeyPoint.Pt.Y));
        }
        var sourcePointsInput = InputArray.Create(sourcePoints);
        var destinationPointsInput = InputArray.Create(destinationPoints);
        
        return Cv2.FindHomography(sourcePointsInput, destinationPointsInput, HomographyMethods.Ransac, minimumNumberOfMatches);

    }

    /// <summary>
    /// Matches features extracted from query and training image and filters out the good matches.
    /// Uses ratio test defined in Lowe's paper (Section 7.1).
    ///     https://people.eecs.berkeley.edu/~malik/cs294/lowe-ijcv04.pdf
    /// </summary>
    /// <param name="queryDescriptors"></param>
    /// <param name="trainDescriptors"></param>
    /// <param name="kRatioThreshold"></param>
    /// <param name="minimumNumberOfMatches"></param>
    /// <returns></returns>
    public static List<DMatch> MatchFeatures(Mat queryDescriptors, Mat trainDescriptors, double kRatioThreshold = 0.75, int minimumNumberOfMatches = 4)
    {
        using var matcher = new FlannBasedMatcher();

        var matches = matcher.KnnMatch(queryDescriptors, trainDescriptors, 2);

        var goodMatches =
            (from match in matches
                where match[0].Distance < kRatioThreshold * match[1].Distance
                select match[0])
            .ToList();
        
        return goodMatches.Count >= minimumNumberOfMatches ? goodMatches : new List<DMatch>();

    }

    public static (Mat, KeyPoint[]) DetectAndComputeDescriptors(Mat queryImage, int maxFeaturesToExtract=500)
    {
        Mat descriptors = new();
        using var detector = SIFT.Create(nFeatures: maxFeaturesToExtract);
        detector.DetectAndCompute(queryImage, null, out var keyPoints, descriptors);
        return (descriptors, keyPoints);
    }

    public static Mat LoadImage(string path, ImreadModes imageReadMode)
    {
        return new Mat(path, imageReadMode);
    }

}