using System.Text;
using ImageMagick;
using MetadataExtractor;
using Microsoft.Extensions.Options;
using OpenCvSharp;
using VinylEye.Core.FeatureExtractors;
using VinylEye.Core.Helpers;
using VinylEye.Core.Model;
using VinylEye.Core.Options;

namespace VinylEye.Core.IO;

public class PhotoSerialiser : IPhotoSerialiser
{
    private const string ReleaseMetadataExifKey = "Copyright";
    private const string DescriptorNodeName = "mat";
    private readonly IFeatureExtractor _featureExtractor;
    private readonly ApplicationOptions _applicationOptions;
 

    public PhotoSerialiser(IFeatureExtractor featureExtractor, IOptions<ApplicationOptions> applicationOptions)
    {
        _featureExtractor = featureExtractor;
        _applicationOptions = applicationOptions.Value;
 
    }

    public (Mat, Size) LoadPhoto(string? fullPath, Size maxProcessingSize, ColorConversionCodes? colorConversionCode = null)
    {
        if (string.IsNullOrWhiteSpace(fullPath) || !File.Exists(fullPath))
            throw new ArgumentException($"Invalid argument. The image file {fullPath} does not exits.",
                nameof(fullPath));

        using var image = new MagickImage(fullPath);

        var originalSize = new Size(image.Width, image.Height);

        // resize if the image size is greater than maxProcessingSize
        var targetSize = CalculateScaledDimensions(originalSize, maxProcessingSize);
        if (targetSize != originalSize)
            image.Resize(targetSize.Width, targetSize.Height);

        if (colorConversionCode == null)
            return (Mat.FromImageData(image.ToByteArray(MagickFormat.Png)), originalSize);

        using var originalImage = Mat.FromImageData(image.ToByteArray(MagickFormat.Png));
        Mat colourConvertedImage = new();
        Cv2.CvtColor(originalImage, colourConvertedImage, colorConversionCode.Value);//ColorConversionCodes.BGR2GRAY

        return (colourConvertedImage, originalSize);
    }

    public byte[] SerialiseDescriptors(Mat mat)
    {
        using var fileStorage = new FileStorage("memory.bin",
            FileStorage.Modes.WriteBase64 | FileStorage.Modes.Memory);

        fileStorage.Write(DescriptorNodeName, mat);
        
        var serialisedDescriptor = fileStorage.ReleaseAndGetString();
        return Encoding.UTF8.GetBytes(serialisedDescriptor);
    }

    public Mat DeSerialiseDescriptors(byte[] data)
    {
        using var fileStorage = new FileStorage(Encoding.UTF8.GetString(data),
            FileStorage.Modes.Read | FileStorage.Modes.Memory);

        using var node = fileStorage[DescriptorNodeName];

        return node == null ? throw new Exception("Teh data does not contain descriptor node.") : node.ReadMat();
    }

    public PhotoFeatures PopulatePhotoInformation(string imagePath)
    {
        var (image, originalDimensions) =
            LoadPhoto(imagePath, _applicationOptions.MaxProcessingDimensions);

        var resizedDims = image.Size();
        var metadataTags = ReadExifTags(imagePath, ReleaseMetadataExifKey);
       
        var (descriptors, _) = _featureExtractor.GetDescriptors(image);
        image.Dispose();
        ReleaseInformation? releaseMetadata = null;
        if (metadataTags.ContainsKey(ReleaseMetadataExifKey))
        {
            releaseMetadata = JsonHelper.DeSerialize<ReleaseInformation>(
                metadataTags.First(x => x.Key.Contains(ReleaseMetadataExifKey, StringComparison.InvariantCultureIgnoreCase))
                    .Value);
        }

        return new PhotoFeatures
        {
            PhotoInformation = new PhotoInfo(imagePath)
            {
                Width = originalDimensions.Width,
                Height = originalDimensions.Height,
                ScaledDownWidth = resizedDims.Width,
                ScaledDownHeight = resizedDims.Height,
                ReleaseMetadata = releaseMetadata
            },
            Descriptors = descriptors
        };
    }

    private static Dictionary<string, string> ReadExifTags(string imagePath, params string[] metadataKeys)
    {
        var directories = ImageMetadataReader.ReadMetadata(imagePath).ToList();

        var metadataTags = (from tag in directories.SelectMany(x => x.Tags)
                join key in metadataKeys on tag.Name equals key
                select new { tag.Name, tag.Description })
            .ToDictionary(x => x.Name, x => x.Description);

        return metadataTags;
    }
    private static Size CalculateScaledDimensions(Size original, Size? maxDims)
    {
        var width = original.Width;
        var height = original.Height;
        var ratio = 1.0;

        if (maxDims.HasValue && width > maxDims.Value.Width)
        {
            ratio = (double)width / maxDims.Value.Width;
        }
        else if (maxDims.HasValue && height > maxDims.Value.Height)
        {
            ratio = (double)height / maxDims.Value.Height;
        }

        return new Size(original.Width / ratio, original.Height / ratio);

    }
}