using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LivingSuburb.Web.Data;
using LivingSuburb.Models.Account;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;

namespace LivingSuburb.Web.Controllers.MobileAPI
{
    [Route("mobileapi/account")]
    [ApiController]
    public class MobAccountController : ControllerBase
    {
        private string securityKey;
        private string validIssuer;
        private string validAudience;
        private readonly IServiceProvider serviceProvider;
        private IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public MobAccountController(IServiceProvider serviceProvider,IConfiguration configuration)
        {
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
            securityKey = this.configuration.GetSection("JWTAuthentication")["SecretKey"].ToString();
            validIssuer = this.configuration.GetSection("JWTAuthentication")["ValidIssuer"].ToString();
            validAudience = this.configuration.GetSection("JWTAuthentication")["ValidAudience"].ToString();
            userManager = (UserManager<ApplicationUser>)this.serviceProvider.GetService(typeof(UserManager<ApplicationUser>));
            signInManager = (SignInManager<ApplicationUser>)this.serviceProvider.GetService(typeof(SignInManager<ApplicationUser>));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("test")]
        public IActionResult Test()
        {
            return Ok("Hello");
        }

        [AllowAnonymous()]
        [HttpPost("login")]
        public IActionResult Login(Login model)
        {
            Task<ApplicationUser> user = userManager.FindByEmailAsync(model.Email);
            if(user.Result != null)
            {
                Task<Microsoft.AspNetCore.Identity.SignInResult> signInResult = signInManager.CheckPasswordSignInAsync(user.Result, model.Password, false);
                if (signInResult.Result.Succeeded)
                {
                    Task<IList<string>> roles = userManager.GetRolesAsync(user.Result);
                    List<Claim> claims = new List<Claim>();
                    foreach (string role in roles.Result)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "User"));
                    }

                    byte[] arrayOfBytes = Encoding.UTF8.GetBytes(securityKey);
                    SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(arrayOfBytes);
                    SigningCredentials signinCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer: validIssuer,
                        audience: validAudience,
                        expires: DateTime.Now.AddDays(1),
                        claims: new List<Claim>() {
                        new Claim(ClaimTypes.Role,"User")
                        },
                        signingCredentials: signinCredentials
                    );

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Wrong email or password");
                }
            }
            else
            {
                return BadRequest("Wrong email or password");
            }
        }
    }
}