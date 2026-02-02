using AutoMapper;
using UcarMobileApi.Application.Common.Interfaces;
using UcarMobileApi.Application.DTOs.Files;
using UcarMobileApi.Core.Entities.Storage;

namespace UcarMobileApi.Application.Mapping;

/// <summary>
/// AutoMapper profile for StoredFile entity mapping.
/// </summary>
public class StoredFileProfile : Profile
{
    public StoredFileProfile()
    {
        CreateMap<StoredFile, StoredFileDto>();

        CreateMap<StoredFileMetadata, StoredFile>();
    }
}
