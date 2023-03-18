using OpenCvSharp;
using ProtoBuf;

namespace VinylEye.Core.Model;

[ProtoContract]
public class PhotoFeatures: IDisposable
{
    [ProtoMember(1)]
    public PhotoInfo? PhotoInformation { get; set; }
    [ProtoMember(2, OverwriteList = true)]
    public byte[]? DescriptorsToSave { get; set; }
    [ProtoIgnore]
    public Mat? Descriptors { get; set; }

    public void Dispose()
    {
        Descriptors?.Dispose();
    }
}