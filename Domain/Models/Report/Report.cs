using Microsoft.WindowsAzure.Storage.Table;

namespace Domain.models;

public class Report : TableEntity
{
    public Report() {}

    public Report(Report reportToCopy)
    {
        ReporterEmail = reportToCopy.ReporterEmail;
        Description = reportToCopy.Description;
        Street = reportToCopy.Street;
        Status = reportToCopy.Status;
        Problem = reportToCopy.Problem;
        ReportDate = reportToCopy.ReportDate;

        PartitionKey = reportToCopy.PartitionKey;
        RowKey = reportToCopy.RowKey;
        Timestamp = reportToCopy.Timestamp;
        ETag = reportToCopy.ETag;
    }

    public Report(string reporterEmail, ProblemType problemType, string description, Address address)
    {
        ReporterEmail = reporterEmail;
        Problem = problemType.ToString();
        Description = description;
        ReportDate = DateTime.Now;
        Status = ProblemStatus.New.ToString();
        Street = address.Street.ToString();

        PartitionKey = address.City.ToString();
        RowKey = $"{ReporterEmail}_{ReportDate:yyyyMMddTHHmmss}";
    }

    public string? ReporterEmail { get; set; }
    public string? Description { get; set; }
    public string? Street { get; set; }
    public string? Status { get; set; }
    public string? Problem { get; set; }
    public DateTime ReportDate { get; set; }
}