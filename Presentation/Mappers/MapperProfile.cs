using AutoMapper;
using Domain.Entities;
using Presentation.DTOs;

namespace Presentation.Mappers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<CreateComplaintDto, Complaint>();
    }
}