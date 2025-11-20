using AutoMapper;
using Product_Management_API.DTOs;
using Product_Management_API.Entities;

namespace Product_Management_API.Mapping;

/// <summary>
/// Basic mapping profile for Product entity to DTO.
/// Maps simple properties without complex transformations.
/// </summary>
public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<CreateProductProfileRequest, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<Product, ProductProfileDto>();
    }
}

