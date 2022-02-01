using Play.Identity.Service.Areas.Identity.Entites;
using Play.Identity.Service.DTOs;

namespace Play.Identity.Service
{
    public static class Extensions
    {
        public static UserDTO AsDTO(this ApplicationUser user)
        {
            return new UserDTO(user.Id, user.UserName, user.Email, user.Gil, user.CreatedOn);
        }
    }
}