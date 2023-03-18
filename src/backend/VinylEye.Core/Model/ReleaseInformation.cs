namespace VinylEye.Core.Model;

public record struct ReleaseInformation(string Title, string ArtistName, string CoverArtUrl, Guid ReleaseId, Guid ArtistId);