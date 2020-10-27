using AutoMapper;
using DatingAPI.Dtos;
using DatingAPI.Models;

namespace DatingAPI.Helpers
{
  public class AutoMapperProfiles : Profile
  {
    public AutoMapperProfiles()
    {
      CreateMap<UserModel, UserForListDto>()
          .ForMember(dest => dest.Age, opt =>
          {
            opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
          });
      CreateMap<UserModel, UserForDetailedDto>()
          .ForMember(dest => dest.Age, opt =>
          {
            opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
          });
          //.ForMember(dest => true);
      CreateMap<UserForUpdateDto, UserModel>();
      CreateMap<UserForRegisterDto, UserModel>();
      CreateMap<PhotoForCreationDto, PhotoModel>();
    }
  }

}
