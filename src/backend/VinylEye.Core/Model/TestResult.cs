using CsvHelper.Configuration.Attributes;

namespace VinylEye.Core.Model;

public record TestResult([Index(0)] string QueryArtist, [Index(1)] string QueryTitle,
    [Index(2)] string MatchedArtist, [Index(3)] string MatchedTitle, [Index(4)] string Duration, [Index(5)] string MaxSize,
    [Index(6)] string BestMatches, [Index(7)] string QueryFile, [Index(8)] string MatchedFile);