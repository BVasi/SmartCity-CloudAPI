using Microsoft.WindowsAzure.Storage.Table;

namespace Domain.models;

public class User : TableEntity //to do: add token/s to DB
{
    public User() {}

    public User(string firstName, string lastName, string email,
        string phoneNumber, string passwordHash, CityName residenceCity)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        PasswordHash = passwordHash;
        PointsNumber = STARTING_POINTS_NUMBER;
        Type = UserType.User.ToString();

        PartitionKey = residenceCity.ToString();
        RowKey = email;
    }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PasswordHash { get; set; }
    public int PointsNumber { get; set; }
    public string? Type { get; set; }

    private const int STARTING_POINTS_NUMBER = 0;
}