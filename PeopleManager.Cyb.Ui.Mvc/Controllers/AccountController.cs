using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PeopleManager.Cyb.Ui.Mvc.Models;
using PeopleManager.Sdk;
using PeopleManager.Services.Model.Requests;
using Vives.Security.Model.Abstractions;

namespace PeopleManager.Cyb.Ui.Mvc.Controllers
{
    public class AccountController(
        IdentitySdk identitySdk,
        ITokenStore tokenStore) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> SignIn(string? returnUrl)
        {
            await HttpContext.SignOutAsync();
            
            ViewBag.ReturnUrl = returnUrl ?? "/";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInModel model, string? returnUrl)
        {

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = "/";
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var signInRequest = new UserSignInRequest
            {
                UserName = model.Username,
                Password = model.Password
            };
            var signInResult = await identitySdk.SignIn(signInRequest);

            if (!signInResult.IsSuccessful)
            {
                ModelState.AddModelError("", "User/Password combination is wrong.");
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            tokenStore.SaveToken(signInResult.Token);
            var principal = CreatePrincipalFromToken(signInResult.Token);
            await HttpContext.SignInAsync(principal);

            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = "/";
            }

            tokenStore.SaveToken(string.Empty);
            await HttpContext.SignOutAsync();

            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? "/";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model, string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = "/";
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var request = new UserRegisterRequest
            {
                UserName = model.Username, 
                Password = model.Password
            };

            var result = await identitySdk.Register(request);

            if (!result.IsSuccessful)
            {
                foreach (var error in result.Messages)
                {
                    ModelState.AddModelError("", error.Message);
                }
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            tokenStore.SaveToken(result.Token);
            var principal = CreatePrincipalFromToken(result.Token);
            await HttpContext.SignInAsync(principal);

            return LocalRedirect(returnUrl);
        }

        private ClaimsPrincipal CreatePrincipalFromToken(string? bearerToken)
        {
            var identity = CreateIdentityFromToken(bearerToken);

            return new ClaimsPrincipal(identity);
        }

        private ClaimsIdentity CreateIdentityFromToken(string? bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return new ClaimsIdentity();
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(bearerToken);
            
            var claims = new List<Claim>();
            foreach (var claim in token.Claims)
            {
                claims.Add(claim);
            }

            //HttpContext required a "Name" claim to display a User Name
            var usernameClaim = token.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            if (usernameClaim is not null)
            {
                claims.Add(new Claim(ClaimTypes.Name, usernameClaim.Value));
            }

            return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
