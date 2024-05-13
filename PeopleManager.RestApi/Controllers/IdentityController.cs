using Microsoft.AspNetCore.Mvc;
using PeopleManager.RestApi.Services;
using PeopleManager.Services.Model.Requests;

namespace PeopleManager.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IdentityService _identityService;

        public IdentityController(IdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(UserSignInRequest request)
        {
            var result = await _identityService.SignIn(request);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            var result = await _identityService.Register(request);
            return Ok(result);
        }
    }
}
