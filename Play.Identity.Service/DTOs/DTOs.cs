using System.ComponentModel.DataAnnotations;

namespace Play.Identity.Service.DTOs
{
    public record UserDTO (Guid Id, string Username, string Email, decimal Gil, DateTimeOffset CreatedDate);

    public record UpdateUserDTO([Required][EmailAddress]string Email, [Range(0.0, 100_000_000)]decimal Gil);
}