using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LivingSuburb.Web.Data;
using LivingSuburb.Models;
using LivingSuburb.Models.Account;
using LivingSuburb.Services;
using LivingSuburb.Services.Email;

namespace LivingSuburb.Web.Controllers
{
    [Route("Account")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly TemplateService _templateService;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly TempService _tempService;
        private readonly string _baseURL;
        public AccountController(
            IConfiguration _configuration,
            IHostingEnvironment _hostingEnvironment,
            TemplateService _templateService,
            UserManager<ApplicationUser> _userManager, 
            SignInManager<ApplicationUser> _signInManager, 
            ILogger<AccountController> logger,
            TempService _tempService)
        {
            this._configuration = _configuration;
            this._tempService = _tempService;
            this._hostingEnvironment = _hostingEnvironment;
            this._templateService = _templateService;
            this._signInManager = _signInManager;
            this._userManager = _userManager;
            this._baseURL = this._configuration.GetSection("BaseURL").Value;
            this._logger = logger;
        }


        [Route("Logout")]
        [HttpGet]
        public IActionResult Logout(string returnUrl)
        {
            _signInManager.SignOutAsync();
            string ReturnUrl = string.IsNullOrEmpty(returnUrl) ? Url.Content("~/") : returnUrl;
            return RedirectPermanent(ReturnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback([FromQuery]string returnUrl = null, [FromQuery]string remoteError = null)
        {
            _logger.LogInformation("ExternalLoginCallback starts");
            _logger.LogInformation("returnUrl = " + returnUrl);
            returnUrl = returnUrl ?? "/";
            _logger.LogInformation("returnUrl = " + returnUrl);
            try
            {
                ExternalLoginInfo loginInfo = await _signInManager.GetExternalLoginInfoAsync();
                _logger.LogInformation("loginInfo.Result is " + (loginInfo == null ? "NULL" : "Not NULL"));
                _logger.LogInformation("loginInfo.LoginProvider => " + loginInfo.LoginProvider);
                _logger.LogInformation("loginInfo.ProviderKey => " + loginInfo.ProviderKey);
                if (loginInfo != null)
                {
                    _logger.LogInformation("loginInfo != null");
                    Claim claim = loginInfo.Principal.Claims.Where(w => w.Type == ClaimTypes.Email).FirstOrDefault();
                    if (claim != null)
                    {
                        _logger.LogInformation("claim != null");
                        string email = claim.Value;
                        _logger.LogInformation("email => " + email);
                        ApplicationUser user = await _userManager.FindByEmailAsync(email);
                        if (user != null)
                        {
                            _logger.LogInformation("user != null");
                            IdentityResult addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
                        }
                        else
                        {
                            _logger.LogInformation("user == null");
                            user = new ApplicationUser();
                            user.Email = email;
                            user.UserName = email;
                            user.Fullname = email;

                            IdentityResult createUserResult = await _userManager.CreateAsync(user, "Password123**");
                            if (createUserResult.Succeeded)
                            {
                                _logger.LogInformation("createUserResult.Succeeded is true");
                                await _userManager.AddToRoleAsync(user, "User");
                                await _userManager.AddClaimsAsync(user, UserDefaultClaims);
                                await _userManager.AddClaimAsync(user, new Claim("Avatar", this._baseURL + "/Avatars/default.jpg"));
                                await _userManager.AddClaimAsync(user, new Claim("Fullname", user.Fullname));
                                user.EmailConfirmed = true;
                                await _userManager.UpdateAsync(user);

                                IdentityResult addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
                                if(addLoginResult.Succeeded)
                                {
                                    _logger.LogInformation("addLoginResult.Succeeded is true");
                                    await _signInManager.SignInAsync(user, true);
                                    return LocalRedirect("/");
                                }
                                else
                                {
                                    _logger.LogInformation("addLoginResult.Succeeded is false");
                                }
                            }
                            else
                            {
                                _logger.LogInformation("createUserResult.Succeeded is false");
                            }
                        }
                    }
                    else
                    {
                        _logger.LogInformation("claim == null");
                        foreach (Claim item in loginInfo.Principal.Claims)
                        {
                            _logger.LogInformation("item.Type => " + item.Type + " item.Value => " + item.Value);
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("loginInfo == null");
                }

                Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true);
                if (signInResult.Succeeded)
                {
                    _logger.LogInformation("signInResult.Succeeded");
                }
                else if (signInResult.IsLockedOut)
                {
                    _logger.LogInformation("signInResult.IsLockedOut");
                    LocalRedirectPermanent("/Account/Lockout");
                }
                else if (signInResult.IsNotAllowed)
                {
                    _logger.LogInformation("signInResult.IsNotAllowed");
                    LocalRedirectPermanent("/Account/NotAllowed");
                }
                else if (signInResult.RequiresTwoFactor)
                {
                    _logger.LogInformation("signInResult.RequireTwoFactor");
                    LocalRedirectPermanent("/Account/RequireTwoFactor");
                }
                else
                {
                    _logger.LogInformation("All loose");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Nih => " + ex.GetBaseException().ToString());
            }
            _logger.LogInformation("ExternalLoginCallback stops");
            return LocalRedirect(returnUrl);
        }

        public virtual AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null)
        {
            _logger.LogInformation("ConfigureExternalAuthenticationProperties starts");
            AuthenticationProperties authenticationProperties = new AuthenticationProperties()
            {
                RedirectUri = redirectUrl
            };
            authenticationProperties.Items["LoginProvider"] = provider;
            _logger.LogInformation("ConfigureExternalAuthenticationProperties stops");
            return authenticationProperties;
        }

        [Route("ExternalLogin")]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            _logger.LogInformation("ExternalLogin starts");
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            _logger.LogInformation("ExternalLogin stops");
            return Challenge(properties, provider);
        }

        private IList<AuthenticationScheme> GetExternalLogins()
        {
            Task<IEnumerable<AuthenticationScheme>> logins = Task.Run(async () => { return await _signInManager.GetExternalAuthenticationSchemesAsync(); });
            return logins.Result.ToList();
        }


        [Route("Email-Verification/{guid}")]
        [HttpGet]
        public IActionResult Verification(string guid)
        {
            try
            {
                string serialized = this._tempService.Read(guid);
                if(string.IsNullOrEmpty(serialized))
                {
                    return RedirectPermanent("/ERROR/400");
                }
                TokenStore tokenStore = JsonConvert.DeserializeObject<TokenStore>(serialized);
                Task<ApplicationUser> user = _userManager.FindByEmailAsync(tokenStore.Email);
                Task<bool> status = _userManager.VerifyUserTokenAsync(user.Result, _userManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation", tokenStore.Token);
                if (status.Result)
                {
                    user.Result.EmailConfirmed = true;
                    _userManager.UpdateAsync(user.Result);
                }
                TempData["Result"] = status.Result ? "Your email has been verified successfuly" : "Invalid or expired email verification token";
            }
            catch (Exception ex)
            {
                TempData["Label"] = "INTERNAL SERVER ERROR";
                return RedirectPermanent("/ERROR/500");
            }
            return View();
        }

        [Route("Reset-Password/{guid}")]
        [HttpPost]
        public IActionResult ResetPassword([FromRoute] string guid,[FromForm] ResetPasswordModel model)
        {
            if(model == null) return RedirectPermanent("/ERROR/400");
            if(ModelState.IsValid)
            {
                if(model.Password != model.ConPassword)
                {
                    ModelState.AddModelError("ConPassword", "Password and Confirming Password must match");
                    return View(model);
                }

                try
                {
                    string serialized = this._tempService.Read(guid);
                    if (string.IsNullOrEmpty(serialized))
                        return RedirectPermanent("/ERROR/400");
                    TokenStore tokenStore = JsonConvert.DeserializeObject<TokenStore>(serialized);
                    Task<ApplicationUser> user = _userManager.FindByEmailAsync(model.Email);
                    if (user.Result == null)
                        return RedirectPermanent("/ERROR/400");
                    Task<IdentityResult> result = _userManager.ResetPasswordAsync(user.Result, tokenStore.Token, model.Password);
                    if (result.Result.Succeeded)
                    {
                        return RedirectPermanent("/ACCOUNT/AFTER-RESET-PASSWORD");
                    }
                    else
                    {
                        TempData["Label"] = "INTERNAL SERVER ERROR";
                        return RedirectPermanent("/ERROR/500");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Label"] = "INTERNAL SERVER ERROR";
                    return RedirectPermanent("/ERROR/500");
                }
            }
            return View(model);
        }

        [Route("Reset-Password/{guid}")]
        [HttpGet]
        public IActionResult ResetPassword(string guid)
        {
            ResetPasswordModel model = new ResetPasswordModel();
            try
            {
                string serialized = this._tempService.Read(guid);
                if (string.IsNullOrEmpty(serialized))
                {
                    return RedirectPermanent("/ERROR/400");
                }
                TokenStore tokenStore = JsonConvert.DeserializeObject<TokenStore>(serialized);
                Task<ApplicationUser> user = _userManager.FindByEmailAsync(tokenStore.Email);
                Task<bool> status = _userManager.VerifyUserTokenAsync(user.Result, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", tokenStore.Token);
                if (status.Result)
                {
                    model.Email = tokenStore.Email;
                }
                else
                {
                    return RedirectPermanent("/ERROR/400");
                }
            }
            catch (Exception ex)
            {
                TempData["Label"] = "INTERNAL SERVER ERROR";
                return RedirectPermanent("/ERROR/500");
            }
            return View(model);
        }

        [Route("Lockout")]
        [HttpGet]
        public IActionResult Lockout()
        {

            return View();
        }

        [Route("NotAllowed")]
        [HttpGet]
        public IActionResult NotAllowed()
        {

            return View();
        }

        [Route("RequireTwoFactor")]
        [HttpGet]
        public IActionResult RequireTwoFactor()
        {

            return View();
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login(string ReturnURL)
        {
            _logger.LogDebug("Login starts at " + Environment.TickCount);
            HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            InputAfterPostModel postModel = new InputAfterPostModel();
            postModel.ExternalLogins = GetExternalLogins();
            postModel.ReturnUrl = ReturnURL ?? Url.Content("~/");
            _logger.LogDebug("Login stops at " + Environment.TickCount);
            return View(postModel);
        }

        [Route("Login")]
        [HttpPost]
        public IActionResult Login(InputAfterPostModel model)
        {
            string ReturnUrl = string.IsNullOrEmpty(model.ReturnUrl) ? Url.Content("~/") : model.ReturnUrl;
            if (ModelState.IsValid)
            {
                Task<Microsoft.AspNetCore.Identity.SignInResult> signInResult = Task.Run(async () => { return await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true); });
                if (signInResult.Result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(ReturnUrl);
                }
                if (signInResult.Result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return LocalRedirectPermanent("/Account/Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            InputAfterPostModel postModel = new InputAfterPostModel(model);
            postModel.ExternalLogins = GetExternalLogins();
            return View(postModel);
        }

        [Route("Register")]
        [HttpGet]
        public IActionResult Register(string ReturnURL)
        {
            _logger.LogDebug("Register starts at " + Environment.TickCount);
            HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            RegisterModel postModel = new RegisterModel();
            postModel.ExternalLogins = GetExternalLogins();
            postModel.ReturnUrl = ReturnURL ?? Url.Content("~/");
            _logger.LogDebug("Register stops at " + Environment.TickCount);
            return View(postModel);
        }

        [Route("Register")]
        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            _logger.LogDebug("Register starts at " + Environment.TickCount);
            string ReturnUrl = string.IsNullOrEmpty(model.ReturnUrl) ? Url.Content("~/") : model.ReturnUrl;
            if (ModelState.IsValid)
            {
                _logger.LogDebug("ModelState.IsValid" + Environment.TickCount);
                Task<ApplicationUser> user = _userManager.FindByEmailAsync(model.Email);
                if (user.Result != null)
                {
                    _logger.LogDebug("user.Result != null" + Environment.TickCount);
                    ModelState.AddModelError("Email", "This email had been used.");
                }
                else
                {
                    _logger.LogDebug("user.Result == null" + Environment.TickCount);
                    if (model.Password != model.ConPassword)
                    {
                        ModelState.AddModelError("ConPassword", "Password and confirm password must be the same.");
                    }
                    else
                    {
                        ApplicationUser newUser = new ApplicationUser();
                        newUser.Fullname = model.Fullname;
                        newUser.Email = model.Email;
                        newUser.UserName = model.Email;
                        newUser.URLAvatar = _baseURL + "/avatars/default.jpg";
                        Task<IdentityResult> registerResult = _userManager.CreateAsync(newUser, model.Password);
                        if (registerResult.Result.Succeeded)
                        {
                            _logger.LogDebug("registerResult.Result.Succeeded" + Environment.TickCount);
                            _logger.LogDebug("UserId = " + newUser.Id);

                            Task<IdentityResult> addRoleResult = _userManager.AddToRoleAsync(newUser, "User");
                            if(!addRoleResult.Result.Succeeded)
                            {
                                _logger.LogDebug("!addRoleResult.Result.Succeeded");
                                foreach (IdentityError item in addRoleResult.Result.Errors)
                                {
                                    _logger.LogDebug(item.Description);
                                }
                            }
                            Task<IdentityResult> addClaimResult = _userManager.AddClaimsAsync(newUser, UserDefaultClaims);
                            if (!addClaimResult.Result.Succeeded)
                            {
                                _logger.LogDebug("!addClaimResult.Result.Succeeded");
                                foreach (IdentityError item in addRoleResult.Result.Errors)
                                {
                                    _logger.LogDebug(item.Description);
                                }
                            }

                            Task<IdentityResult> addFullnameResult = _userManager.AddClaimAsync(newUser, new Claim("Fullname", model.Fullname));
                            if (!addFullnameResult.Result.Succeeded)
                            {
                                _logger.LogDebug("!addAvatarResult.Result.Succeeded");
                            }

                            Task<IdentityResult> addAvatarResult = _userManager.AddClaimAsync(newUser, new Claim("Avatar",this._baseURL + "/Avatars/default.jpg"));
                            if(!addAvatarResult.Result.Succeeded)
                            {
                                _logger.LogDebug("!addAvatarResult.Result.Succeeded");
                            }

                            Task<string> confirmationToken = _userManager.GenerateUserTokenAsync(newUser,_userManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation");

                            TokenStore tokenStore = new TokenStore();
                            tokenStore.Email = model.Email;
                            tokenStore.Token = confirmationToken.Result;
                            string serialized = JsonConvert.SerializeObject(tokenStore);
                            string guid = this._tempService.Create(serialized);
                            try
                            {
                                AfterRegistration registrationEmail = new AfterRegistration(this._templateService, this._configuration, this._hostingEnvironment);
                                registrationEmail.Email = model.Email;
                                registrationEmail.Fullname = model.Fullname;
                                registrationEmail.Token = guid;
                                registrationEmail.Send();
                                _logger.LogDebug("After registration email has been sent to : " + model.Email);
                                return LocalRedirect("/Account/After-Registration");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Error after trying to send registration email : " + ex.GetBaseException().ToString());
                                return LocalRedirect("/Account/After-Registration");
                            }
                        }
                        else
                        {
                            foreach (IdentityError item in registerResult.Result.Errors)
                            {
                                ModelState.AddModelError("Password", item.Description);
                                ModelState.AddModelError("ConPassword", item.Description);
                                break;
                            }
                        }
                    }
                }
            }


            RegisterModel postModel = new RegisterModel(model);
            postModel.ExternalLogins = GetExternalLogins();
            _logger.LogDebug("Register stops at " + Environment.TickCount);
            return View(postModel);
        }


        
        [Route("After-Registration")]
        [HttpGet]
        public IActionResult AfterRegister()
        {
            return View();
        }

        [Route("After-Reset-Password")]
        [HttpGet]
        public IActionResult AfterResetPassword()
        {

            return View();
        }

        [Route("After-ForgotPassword")]
        [HttpGet]
        public IActionResult AfterForgotPassword()
        {

            return View();
        }

        [Route("ForgotPassword")]
        [HttpGet]
        public IActionResult ForgotPassword()
        {

            return View();
        }

        [Route("ForgotPassword")]
        [HttpPost]
        public IActionResult ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                MatchCollection validation = Regex.Matches(model.Email, @"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,3}");
                if (validation.Count == 0)
                {
                    ModelState.AddModelError("Email", "Invalid email format.");
                }
                else
                {
                    Task<ApplicationUser> user = _userManager.FindByEmailAsync(model.Email);
                    if(user.Result == null)
                    {
                        ModelState.AddModelError("Email", "This email can not be found.");
                    }
                    else
                    {
                        Task<string> confirmationToken = _userManager.GenerateUserTokenAsync(user.Result, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword");
                        TokenStore tokenStore = new TokenStore();
                        tokenStore.Email = user.Result.Email;
                        tokenStore.Token = confirmationToken.Result;

                        string serialized = JsonConvert.SerializeObject(tokenStore);
                        string guid = this._tempService.Create(serialized);
                        try
                        {
                            AfterForgotPassword clientEmail = new AfterForgotPassword(this._templateService, this._configuration, this._hostingEnvironment);
                            clientEmail.Email = user.Result.Email;
                            clientEmail.Fullname = user.Result.Fullname;
                            clientEmail.Token = guid;
                            clientEmail.Send();
                            return LocalRedirect("/Account/After-ForgotPassword");
                        }
                        catch (Exception ex)
                        {
                            string error = ex.GetBaseException().ToString();
                        }
                    }
                }
            }
            return View(model);
        }

        private IList<Claim> UserDefaultClaims {
            get
            {
                return new List<Claim>() {
                    new Claim("CAN_UPDATE_AVATAR", "true"),
                    new Claim("CAN_UPDATE_PROFILE", "true"),
                    new Claim("CAN_POST_JOB", "true"),
                    new Claim("CAN_UPDATE_JOB", "true"),
                    new Claim("CAN_DELETE_JOB", "true"),
                    new Claim("CAN_POST_COMMENT", "true"),
                    new Claim("CAN_UPDATE_COMMENT", "true"),
                    new Claim("CAN_DELETE_COMMENT", "true")
                };
            }
        }
    }
}
