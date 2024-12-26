using Domain.models;
using DTO.models;
using Microsoft.AspNetCore.Mvc;
using services;
using utils;
namespace API.Controllers;

[Route(BASE_ROUTE)]
[ApiController]
public class UsersController : ControllerBase
{
    public UsersController(UserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost(SIGN_UP_ROUTE)]
    public async Task<IActionResult> SignUp(UserSignUpRequest request)
    {
        if (await _userService.GetUserByEmailAsync(request.Email!) != null)
        {
            return Conflict();
        }
        await _userService.AddUserAsync(request);
        var createdUser = await _userService.GetUserAsync(request.ResidenceCity, request.Email!);
        if (createdUser == null)
        {
           return StatusCode(StatusCodes.Status500InternalServerError);
        }
        return CreatedAtAction(nameof(LogIn), new UserLogInResponse(createdUser,
            JwtUtils.GenerateTokenForUser(createdUser, _configuration)));
    }

    [HttpPost(LOG_IN_ROUTE)]
    public async Task<IActionResult> LogIn(UserLogInRequest request)
    {
        var possibleLoggedInUser = await _userService.GetUserByEmailAsync(request.Email!);
        if (possibleLoggedInUser == null)
        {
            return Unauthorized();
        }
        if (!PasswordHasher.CheckPassword(possibleLoggedInUser.PasswordHash!, request.Password!))
        {
            return Unauthorized();
        }
        return Ok(new UserLogInResponse(possibleLoggedInUser,
            JwtUtils.GenerateTokenForUser(possibleLoggedInUser, _configuration)));
    }

    private readonly UserService _userService;
    private readonly IConfiguration _configuration;
    private const string BASE_ROUTE = "api/[controller]";
    private const string SIGN_UP_ROUTE = "signup";
    private const string LOG_IN_ROUTE = "login";
}
