using Domain.models;
namespace DTO.models;

public class GetReportResponse
{
    public GetReportResponse(Report report)
    {
        Id = report.RowKey;
        ReporterEmail = report.ReporterEmail;
        Description = report.Description;
        ProblemAddress = new Address
        {
            Street = Enum.Parse<StreetName>(report.Street!),
            City = Enum.Parse<CityName>(report.PartitionKey)
        };
        ReportDate = report.ReportDate;
        Status = Enum.Parse<ProblemStatus>(report.Status!);
        Problem = Enum.Parse<ProblemType>(report.Problem!);
    }

    public string? Id { get; set; }
    public string? ReporterEmail { get; set; }
    public string? Description { get; set; }
    public Address? ProblemAddress { get; set; }
    public DateTime ReportDate { get; set; }
    public ProblemStatus Status { get; set; }
    public ProblemType Problem { get; set; }
}