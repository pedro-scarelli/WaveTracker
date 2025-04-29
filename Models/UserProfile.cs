namespace LoginApi.Models;

using AutoMapper;

using LoginApi.DTOs;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UpdateUserDTO, User>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
