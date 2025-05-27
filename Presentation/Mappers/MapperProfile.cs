using AutoMapper;
using Domain.Entities;
using Presentation.DTOs;
using Presentation.DTOs.Authority;
using Presentation.DTOs.Submission;

namespace Presentation.Mappers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // CreateDto -> Entity
        CreateMap<CreateSubmissionDto, Submission>();
        CreateMap<CreateAuthorityDto, Authority>();
        
        // Entity -> ReadDto
        CreateMap<Submission, ReadSubmissionDto>();
        CreateMap<Authority, ReadAuthorityDto>()
            .ForMember(dest => dest.ReadSubmissionDtos, opt => opt.MapFrom(src => src.Submissions));
        
        //UpdateDto -> Entity
        CreateMap<UpdateSubmissionDto, Submission>();
        CreateMap<UpdateAuthorityDto, Authority>();
    }
}