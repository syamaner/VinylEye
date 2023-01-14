using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using OpenCvSharp;
using VinylEye.Cli.Helpers;

namespace VinylEye.Cli.Commands;

public class PerspectiveCorrectImageCommand : Command
{
    public PerspectiveCorrectImageCommand()
        : base("perspective-correct", "Matches an image to current index.")
    {
        var outputDirectoryOption = new Option<string>("--output-directory", () => "images", "Where to output images. Can be relative or absolute path.");
        AddOption(outputDirectoryOption);

        Handler = CommandHandler.Create((string outputDirectory) => PerformPerspectiveCorrection(outputDirectory));
    }

    private int PerformPerspectiveCorrection(string outputDirectory)
    {
        EnsureFilesExist(outputDirectory);
        
        var queryImagePath = Path.Combine(outputDirectory, "made_in_europe_query.jpeg");
        var trainImagePath = Path.Combine(outputDirectory, "made_in_europe_train.jpg");

        using var queryImage = ImageHelper.LoadImage(queryImagePath, ImreadModes.Grayscale);
        using var trainImage = ImageHelper.LoadImage(trainImagePath, ImreadModes.Grayscale);
     
        var (queryDescriptors, queryKeyPoints) = ImageHelper.DetectAndComputeDescriptors(queryImage);
        var (trainDescriptors, trainKeyPoints) = ImageHelper.DetectAndComputeDescriptors(trainImage);
        
        var matches = ImageHelper.MatchFeatures(queryDescriptors, trainDescriptors);
        var homographyMatrix = ImageHelper.CalculateHomographyMatrix(queryKeyPoints, trainKeyPoints, matches);

        double width = trainImage.Width + queryImage.Width;
        double height = trainImage.Height + queryImage.Height;

        using var queryImageColour = ImageHelper.LoadImage(queryImagePath, ImreadModes.Color);
        using var unWarpedImage=new Mat();

        if (homographyMatrix == null) return 0;

        Cv2.WarpPerspective(queryImageColour, unWarpedImage,homographyMatrix, new Size(width, height));

        using var croppedAndUnWarpedImage = new Mat(unWarpedImage, new Rect(0, 0, trainImage.Width, trainImage.Height));

        Cv2.ImWrite(Path.Combine(outputDirectory, "query_perspective_corrected.jpeg"), croppedAndUnWarpedImage);
        return 1;
    }
    
    private static void EnsureFilesExist(string outputDirectory)
    {
        FileHelper.EnsureFilesExistsInOutputDirectory(outputDirectory,
            Path.Combine("images", "made_in_europe_query.jpeg"));
        FileHelper.EnsureFilesExistsInOutputDirectory(outputDirectory,
            Path.Combine("images", "made_in_europe_train.jpg"));
        FileHelper.EnsureFilesExistsInOutputDirectory(outputDirectory,
            Path.Combine("images", "the_real_thing.jpg"));
    }
}