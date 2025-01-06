using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using utils;
namespace services;

public class UserService
{
    public UserService(IConfiguration configuration)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString(AZURE_STORAGE));
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        _usersTable = tableClient.GetTableReference(TABLE_NAME);
        _usersTable.CreateIfNotExistsAsync().Wait();
    }

    public async Task AddUserAsync(DTO.models.UserSignUpRequest user)
    {
        await _usersTable.ExecuteAsync(TableOperation.Insert(
            new Domain.models.User(user.FirstName!, user.LastName!,
                user.Email!, user.PhoneNumber!,
                PasswordHasher.HashPassword(user.Password!), user.ResidenceCity)
        ));
    }

    public async Task<Domain.models.User?> GetUserAsync(Domain.models.CityName city, string email)
    {
        TableResult result = await _usersTable.ExecuteAsync(TableOperation.Retrieve<Domain.models.User>(city.ToString(), email));
        return result.Result as Domain.models.User;
    }

    public async Task<Domain.models.User?> GetUserByEmailAsync(string email)
    {
        TableQuery<Domain.models.User> query = new TableQuery<Domain.models.User>()
            .Where(TableQuery.GenerateFilterCondition(ROW_KEY, QueryComparisons.Equal, email));
        var result = await _usersTable.ExecuteQuerySegmentedAsync(query, null);
        return result.Results.FirstOrDefault();
    }

    public async Task<bool> IsUserAdminByEmailAsync(string email)
    {
        TableQuery<Domain.models.User> query = new TableQuery<Domain.models.User>()
            .Where(TableQuery.GenerateFilterCondition(ROW_KEY, QueryComparisons.Equal, email));
        var possibleUser = await _usersTable.ExecuteQuerySegmentedAsync(query, null);
        var user = possibleUser.Results.FirstOrDefault();
        if ((user == null) || (Enum.Parse<Domain.models.UserType>(user.Type!) != Domain.models.UserType.Admin))
        {
            return false;
        }
        return true;
    }

    public async Task<Domain.models.User?> AwardPointsToUserAsync(string email)
    {
        var userToUpdate = await GetUserByEmailAsync(email);
        if (userToUpdate == null)
        {
            return null;
        }
        var updatedUser = new Domain.models.User(userToUpdate);
        updatedUser.PointsNumber += REWARDED_POINTS;
        await _usersTable.ExecuteAsync(TableOperation.Replace(updatedUser));
        return updatedUser;
    }

    private readonly CloudTable _usersTable;
    private const string TABLE_NAME = "Users";
    private const string AZURE_STORAGE = "AzureStorage";
    private const string ROW_KEY = "RowKey";
    private const int REWARDED_POINTS = 10;
}