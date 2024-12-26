using Domain.models;
namespace DTO.models;

public class UserSignUpRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Password { get; set; }
    public CityName ResidenceCity { get; set; }
}
