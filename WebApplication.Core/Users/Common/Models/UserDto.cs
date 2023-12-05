using AutoMapper;
using WebApplication.Core.Common.Interfaces;
using WebApplication.Core.Users.Commands;
using WebApplication.Infrastructure.Entities;

namespace WebApplication.Core.Users.Common.Models
{
    public class UserDto : IMapFrom<User>
    {
        public int UserId { get; set; }
        public string GivenNames { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? MobileNumber { get; set; }
        public string? EmailAddress { get; set; }

        public string FullName => ToString();

        public void Mapping(Profile profile)
        {
            profile.CreateMap<User, UserDto>()
                   .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                   .ForMember(
                       dest => dest.EmailAddress,
                       opt => opt.MapFrom(src => src.ContactDetail!.EmailAddress)
                   )
                   .ForMember(
                       dest => dest.MobileNumber,
                       opt => opt.MapFrom(src => src.ContactDetail!.MobileNumber)
                   );
        }

        public override string ToString()
        {
            return $"{GivenNames} {LastName}";
        }
    }
}
