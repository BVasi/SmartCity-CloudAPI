using Domain.models;
namespace DTO.models;

public class UserLogInResponse
{
    public UserLogInResponse(User user)
    {
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.RowKey;
        PhoneNumber = user.PhoneNumber;
        PointsNumber = user.PointsNumber;
        ResidenceCity = Enum.Parse<CityName>(user.PartitionKey);
        Token = user.Token;
    }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Token { get; set; }
    public int PointsNumber { get; set; }
    public CityName ResidenceCity { get; set; }
}