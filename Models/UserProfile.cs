namespace LoginApi.Models;

using AutoMapper;

using LoginApi.DTOs.Request;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UpdateUserRequestDTO, User>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
