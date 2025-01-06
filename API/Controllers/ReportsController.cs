using Domain.models;
using DTO.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using services;
using utils;
namespace API.Controllers;

[Route(BASE_ROUTE)]
[ApiController]
public class ReportsController : ControllerBase
{
    public ReportsController(ReportService reportService, UserService userService, IConfiguration configuration)
    {
        _reportService = reportService;
        _userService = userService;
        _configuration = configuration;
    }

    [HttpGet(GET_REPORTED_PROBLEM_ROUTE)]
    public async Task<IActionResult> GetReport(CityName cityName, string id)
    {
        var report = await _reportService.GetReportAsync(cityName, id);
        if (report == null)
        {
            return NotFound();
        }
        return Ok(new GetReportResponse(report));
    }

    [HttpGet(GET_CITY_REPORTS)]
    public async Task<IActionResult> GetReports(CityName cityName)
    {
        var reports = await _reportService.GetReportsByCityAsync(cityName);
        if (reports.IsNullOrEmpty())
        {
            return NotFound();
        }
        return Ok(reports.Select(report => new GetReportResponse(report)).ToList());
    }

    [HttpPost(REPORT_PROBLEM_ROUTE)]
    public async Task<IActionResult> ReportProblem(CreateReportRequest request)
    {
        var jwtToken = Request.Headers[AUTHORIZATION_HEADER].ToString()?.Replace(BEARER_PARAMETER, EMPTY_STRING);
        var userEmail = JwtUtils.GetUserEmailFromToken(jwtToken!, _configuration);
        if ((userEmail != request.ReporterEmail) && (!await _userService.IsUserAdminByEmailAsync(userEmail!)))
        {
            return Unauthorized();
        }
        if (!CityStreetValidator.IsValidStreetForCity(request.ProblemAddress!.City!, request.ProblemAddress!.Street!))
        {
            return BadRequest();
        }
        var createdReport = await _reportService.AddReportAsync(request);
        return CreatedAtAction(
            nameof(GetReport),
            new { cityName = (int)Enum.Parse<CityName>(createdReport.PartitionKey), id = createdReport.RowKey },
            new GetReportResponse(createdReport)
        );
    }

    [HttpPatch(CHANGE_REPORT_STATUS_ROUTE)]
    public async Task<IActionResult> ChangeReportStatus(CityName cityName, string id, ChangeReportStatusRequest request)
    {
        var jwtToken = Request.Headers[AUTHORIZATION_HEADER].ToString()?.Replace(BEARER_PARAMETER, EMPTY_STRING);
        var userEmail = JwtUtils.GetUserEmailFromToken(jwtToken!, _configuration);
        if (userEmail == null)
        {
            return Unauthorized();
        }
        if (!await _userService.IsUserAdminByEmailAsync(userEmail))
        {
            return Unauthorized();
        }
        var reportToUpdate = await _reportService.GetReportAsync(cityName, id);
        if (reportToUpdate == null)
        {
            return NotFound();
        }
        var updatedReport = await _reportService.UpdateReportAsync(reportToUpdate, request);
        if ((updatedReport == null) || (updatedReport.Status == reportToUpdate.Status))
        {
            return Conflict();
        }
        if (updatedReport.IsRedeemed && (!reportToUpdate.IsRedeemed))
        {
            if (await _userService.AwardPointsToUserAsync(updatedReport.ReporterEmail!) == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        return Ok(new GetReportResponse(updatedReport));
    }

    [HttpDelete(DELETE_REPORTED_PROBLEM_ROUTE)]
    public async Task<IActionResult> DeleteReport(CityName cityName, string id)
    {
        var jwtToken = Request.Headers[AUTHORIZATION_HEADER].ToString()?.Replace(BEARER_PARAMETER, EMPTY_STRING);
        var userEmail = JwtUtils.GetUserEmailFromToken(jwtToken!, _configuration);
        if (userEmail == null)
        {
            return Unauthorized();
        }
        if (!await _userService.IsUserAdminByEmailAsync(userEmail))
        {
            return Unauthorized();
        }
        var reportToDelete = await _reportService.GetReportAsync(cityName, id);
        if (reportToDelete == null)
        {
            return NotFound();
        }
        await _reportService.DeleteReportAsync(reportToDelete);
        return NoContent();
    }

    private readonly ReportService _reportService;
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;
    private const string BASE_ROUTE = "api/[controller]";
    private const string REPORT_PROBLEM_ROUTE = "reportProblem";
    private const string CHANGE_REPORT_STATUS_ROUTE = "city/{cityName}/problem/{id}";
    private const string GET_REPORTED_PROBLEM_ROUTE = "city/{cityName}/problem/{id}";
    private const string GET_CITY_REPORTS = "city/{cityName}";
    private const string DELETE_REPORTED_PROBLEM_ROUTE = "city/{cityName}/problem/{id}";
    private const string AUTHORIZATION_HEADER = "Authorization";
    private const string BEARER_PARAMETER = "Bearer ";
    private const string EMPTY_STRING = "";
}
