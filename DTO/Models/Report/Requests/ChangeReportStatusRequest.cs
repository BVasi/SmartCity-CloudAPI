using Domain.models;
namespace DTO.models;

public class ChangeReportStatusRequest
{
    public ProblemStatus? NewStatus { get; set; }
}