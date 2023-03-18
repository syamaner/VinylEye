using CsvHelper.Configuration.Attributes;

namespace VinylEye.Core.Model;

public record ReleaseQuery([Index(0)] string FilePath, [Index(1)] string Artist, [Index(2)] string ReleaseUrl, [Index(3)] string AlbumName);