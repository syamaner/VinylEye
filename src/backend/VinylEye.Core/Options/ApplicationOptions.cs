using OpenCvSharp;

namespace VinylEye.Core.Options;

public class ApplicationOptions
{
    public string ApplicationDataDirectory { get; set; } = "C:\\vinylid";
    // Relative to ApplicationDataDirectory
    public string DatabaseDirectoryName { get; set; } = "data\\db";
    public string ImageDownloadDirectoryName { get; set; } = "data\\Images";
    public string IndexDirectoryName { get; set; } = "test\\sample\\index";
    public string ExportedImageDirectoryName { get; set; } = "test\\sample\\exported";
    public string TestDirectoryName = "test\\sample\\input";


    public string IndexFileName = "index_sift_1400_1000.bin";


    public int MaxProcessingImageWidth { get; init; }
    public int MaxProcessingImageHeight { get; init; }
    public Size MaxProcessingDimensions => new(MaxProcessingImageWidth, MaxProcessingImageHeight);

    public int MaxCores { get; set; } = Environment.ProcessorCount;

}