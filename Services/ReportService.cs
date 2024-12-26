using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
namespace services;

public class ReportService
{
    public ReportService(IConfiguration configuration)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString(AZURE_STORAGE));
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        _reportsTable = tableClient.GetTableReference(TABLE_NAME);
        _reportsTable.CreateIfNotExistsAsync().Wait();
    }

    public async Task<Domain.models.Report> AddReportAsync(DTO.models.CreateReportRequest reportRequest)
    {
        var newReport = new Domain.models.Report(reportRequest.ReporterEmail!, reportRequest.Problem,
                reportRequest.Description!, reportRequest.ProblemAddress!);
        await _reportsTable.ExecuteAsync(TableOperation.Insert(newReport));
        return newReport;
    }

    public async Task<Domain.models.Report?> UpdateReportAsync(Domain.models.Report reportToUpdate,
        DTO.models.ChangeReportStatusRequest changeReportStatusRequest)
    {
        var updatedReport = new Domain.models.Report(reportToUpdate);
        updatedReport.Status = changeReportStatusRequest.NewStatus.ToString();
        await _reportsTable.ExecuteAsync(TableOperation.Replace(updatedReport));
        return updatedReport;
    }

    public async Task<Domain.models.Report?> GetReportAsync(Domain.models.CityName city, string id)
    {
        TableResult result = await _reportsTable.ExecuteAsync(TableOperation.Retrieve<Domain.models.Report>(city.ToString(), id));
        return result.Result as Domain.models.Report;
    }

    public async Task<List<Domain.models.Report>> GetReportsByCityAsync(Domain.models.CityName city)
    {
        List<Domain.models.Report> reports = new List<Domain.models.Report>();
        TableQuery<Domain.models.Report> query = new TableQuery<Domain.models.Report>()
            .Where(TableQuery.GenerateFilterCondition(PARTITION_KEY, QueryComparisons.Equal, city.ToString()));
        TableQuerySegment<Domain.models.Report>? segment = null;
        do
        {
            segment = await _reportsTable.ExecuteQuerySegmentedAsync(query, segment?.ContinuationToken);
            reports.AddRange(segment.Results);
        } while (segment.ContinuationToken != null);
        return reports;
    }

    private readonly CloudTable _reportsTable;
    private const string TABLE_NAME = "Reports";
    private const string AZURE_STORAGE = "AzureStorage";
    private const string PARTITION_KEY = "PartitionKey";
}