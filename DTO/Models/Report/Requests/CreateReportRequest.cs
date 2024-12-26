using Domain.models;
namespace DTO.models;

public class CreateReportRequest
{
    public string? ReporterEmail { get; set; }
    public string? Description { get; set; }
    public Address? ProblemAddress { get; set; }
    public ProblemType Problem { get; set; }
}