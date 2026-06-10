using AutoMapper;
using ITAM.Dto;
using ITAM.Models;

namespace ITAM.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Contract, ContractDto>().ReverseMap();
            CreateMap<Vendor, VendorDto>().ReverseMap();
        }
    }
}