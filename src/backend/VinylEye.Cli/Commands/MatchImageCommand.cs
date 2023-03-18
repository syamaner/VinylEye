using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using OpenCvSharp;
using VinylEye.Cli.Helpers;
using VinylEye.Core.Helpers;

// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable once UnusedMember.Global

namespace VinylEye.Cli.Commands;

public class MatchImageCommand : Command
{
    public MatchImageCommand()
        : base("match", "Matches an image to current index.")
    {
        var outputDirectoryOption = new Option<string>("--output-directory", () => "images", "Where to output images. Can be relative or absolute path.");
        AddOption(outputDirectoryOption);
        
        Handler = CommandHandler.Create((string outputDirectory) =>
        {
            MatchImages(outputDirectory, true);
            MatchImages(outputDirectory, false);
        });
    }

    private static int MatchImages(string outputDirectory, bool useCorrectMatches)
    {
        EnsureFilesExist(outputDirectory);
        
        var queryImagePath = Path.Combine(outputDirectory, "made_in_europe_query.jpeg");
        var trainingImagePath = useCorrectMatches
            ? Path.Combine(outputDirectory, "made_in_europe_train.jpg")
            : Path.Combine(outputDirectory, "the_real_thing.jpg");

        var outputFilePath = useCorrectMatches
            ? Path.Combine(outputDirectory, "matching_pair.jpg")
            : Path.Combine(outputDirectory, "non_matching_pair.jpg");
 
        using var queryImage = ImageHelper.LoadImage(queryImagePath, ImreadModes.Grayscale);
        using var trainImage = ImageHelper.LoadImage(trainingImagePath, ImreadModes.Grayscale);

        var (queryDescriptors, queryKeyPoints) = ImageHelper.DetectAndComputeDescriptors(queryImage);
        var (trainDescriptors, trainKeyPoints) = ImageHelper.DetectAndComputeDescriptors(trainImage);

        var matches = ImageHelper.MatchFeatures(queryDescriptors, trainDescriptors,kRatioThreshold:0.5);

        using var colourQueryImage = ImageHelper.LoadImage(queryImagePath, ImreadModes.Color);
        using var colourTrainImage = ImageHelper.LoadImage(trainingImagePath, ImreadModes.Color);

        var imgMatches = new Mat();
        Cv2.DrawMatches(colourQueryImage, queryKeyPoints, colourTrainImage, trainKeyPoints, matches, imgMatches);
        Cv2.ImWrite(outputFilePath, imgMatches);


        return 1;
    }

    private static void EnsureFilesExist(string outputDirectory)
    {
        PathExtensions.EnsureFilesExistsInOutputDirectory(outputDirectory,
            Path.Combine("images", "made_in_europe_query.jpeg"));
        PathExtensions.EnsureFilesExistsInOutputDirectory(outputDirectory,
            Path.Combine("images", "made_in_europe_train.jpg"));
        PathExtensions.EnsureFilesExistsInOutputDirectory(outputDirectory,
            Path.Combine("images", "the_real_thing.jpg"));
    }
}
 