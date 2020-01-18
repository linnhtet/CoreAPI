using CoreAPI.Contracts.v1;
using CoreAPI.Contracts.v1.Request;
using CoreAPI.Contracts.v1.Response;
using CoreAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Controllers.v1
{
    public class IdentityController:Controller
    {
        private readonly IIdentityService _identitySerice;
        public IdentityController(IIdentityService identityService)
        {
            _identitySerice = identityService;
        }

        [HttpPost(ApiRoutes.Identity.SignUp)]
        public async Task<IActionResult> SingUp([FromBody] UserRegistrationRequest userRegistrationRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(err => err.Errors.Select(e=>e.ErrorMessage))
                });
            }
            var authResponse = await _identitySerice.RegisterAsync(userRegistrationRequest.Email, userRegistrationRequest.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse { 
                    Errors=authResponse.Errors
                });;
            }
            return  Ok(new AuthSuccessResponse { Token=authResponse.Token });
        }
        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
        {
            var authResponse = await _identitySerice.LoginAsync(userLoginRequest.Email, userLoginRequest.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                }); ;
            }
            return Ok(new AuthSuccessResponse { Token = authResponse.Token });
        }
    }
}
 