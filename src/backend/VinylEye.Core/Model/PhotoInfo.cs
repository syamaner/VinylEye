using ProtoBuf;

namespace VinylEye.Core.Model;

[ProtoContract]
public class PhotoInfo
{
    public PhotoInfo() { }
    public PhotoInfo(string? imagePath)
    {
        ImagePath = imagePath;
    }
    [ProtoMember(1)]
    private string? _imagePath;
    [ProtoMember(2)]
    public int Width { get; set; }
    [ProtoMember(3)]
    public int Height { get; set; }
    [ProtoMember(4)]
    public int ScaledDownWidth { get; set; }
    [ProtoMember(5)]
    public int ScaledDownHeight { get; set; }

    [ProtoMember(6)]
    public string? FileType { get; set; }
    [ProtoMember(7)]
    public string? FileName { get; set; }

    [ProtoMember(8)]
    public string? ImagePath
    {
        get => _imagePath;
        set
        {
            if (value == null || value.Equals(_imagePath, StringComparison.OrdinalIgnoreCase)) return;

            FileName = Path.GetFileName(value);
            FileType = Path.GetExtension(value);
            _imagePath = value;
        }
    }

    [ProtoIgnore]
    public ReleaseInformation? ReleaseMetadata { get; set; }
 
}