using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebAPI.Data;
using MyWebAPI.Helpers;
using MyWebAPI.Models;
using MyWebAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyWebAPI.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly SignInManager<ApiUser> _sighiInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthManager _authManager;
        private readonly IMapper _mapper;

        public AccountController(UserManager<ApiUser> userManager,
                                 SignInManager<ApiUser> signInManager,
                                 ILogger<AccountController> logger,
                                 IAuthManager authManager,
                                 IMapper mapper)
        {
            _userManager = userManager;
            _sighiInManager = signInManager;
            _logger = logger;
            _authManager = authManager;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserDTO model)
        {
            _logger.LogInformation($"Registration attampt for {model.Email}");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var user = _mapper.Map<ApiUser>(model);
                user.UserName = model.Email;

                // adding user
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    return BadRequest(ModelState);
                }

                // addting role to the user after receiving success
                await _userManager.AddToRolesAsync(user, new List<string> { Role.User.ToString() });

                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{Constants.SomethingWrong} {nameof(Register)}");
                return Problem($"{Constants.SomethingWrong} {nameof(Register)}", statusCode: 500);
            }
        }

        /*

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            _logger.LogInformation($"Registration attampt for {model.Email}");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _sighiInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (!result.Succeeded) return Unauthorized("Invalid login attampt");

                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{Constants.SomethingWrong} {nameof(Register)}");
                return Problem($"{Constants.SomethingWrong} {nameof(Register)}", statusCode: 500);
            }
        }

        */
    }
}
