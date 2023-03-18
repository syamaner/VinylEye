using OpenCvSharp;
using VinylEye.Core.Model;

namespace VinylEye.Core.IO;

public interface IPhotoSerialiser
{
    PhotoFeatures PopulatePhotoInformation(string imagePath);
    (Mat, Size) LoadPhoto(string? fullPath, Size maxProcessingSize, ColorConversionCodes? colorConversionCode = null);
    public byte[] SerialiseDescriptors(Mat mat);
    public Mat DeSerialiseDescriptors(byte[] data);
}