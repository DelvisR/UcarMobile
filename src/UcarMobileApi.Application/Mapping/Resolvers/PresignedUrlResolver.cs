using AutoMapper;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Files;
using UcarMobileApi.Core.Entities.Storage;

namespace UcarMobileApi.Application.Mapping.Resolvers;

/// <summary>
/// Resolves a presigned S3 URL for a stored file using the configured storage service.
/// </summary>
public class PresignedUrlResolver(IFileStorageService storageService) : IValueResolver<StoredFile, StoredFileDto, string?>
{
    public string? Resolve(StoredFile source, StoredFileDto destination, string? destMember, ResolutionContext context)
    {
        return string.IsNullOrWhiteSpace(source.Key) ? null :
            // Direct call since presigning is a local computation (no AWS call)
            storageService.GenerateDownloadPresignedUrl(source.Key, source.Bucket);
    }
}
