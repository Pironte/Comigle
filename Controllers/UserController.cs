using Comigle.Model.Dtos;
using Comigle.Services;
using ComigleApi.Model.Email;
using ComigleApi.Model.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comigle.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class UserController : ControllerBase
    {
        public UserService _userService { get; set; }

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("estou vivo");
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(CreateUserDto createUserDto)
        {
            var response = await _userService.Register(createUserDto);
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            var response = await _userService.Login(loginUserDto);
            return Ok(response);
        }

        [HttpPost("SendEmailToResetPassword")]
        public async Task<IActionResult> SendEmailToResetPassword(SendEmailRequest sendEmailRequest)
        {
            var response = await _userService.SendEmailToResetPassword(sendEmailRequest);
            return Ok(response);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var response = await _userService.ResetPassword(resetPasswordRequest);
            return Ok(response);
        }

        [HttpPost("LogOut")]
        [Authorize]
        public IActionResult LogOut()
        {
            _userService.LogOut();
            return Ok();
        }
    }
}
