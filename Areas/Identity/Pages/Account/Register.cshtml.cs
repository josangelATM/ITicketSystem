using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using ITicketSystem.Models;
using ITicketSystem.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ITicketSystem.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly INotyfService _notyf;


        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            INotyfService notyf)
        {
            _notyf = notyf;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
  
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Employee Number")]
            public int EmployeeNumber { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string Lastname { get; set; }

            [Required]
            public string Position { get; set; }

            public string Role { get; set; }


            [Required]
            public string PhoneNumber { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }
            


        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            Input = new InputModel()
            {
                RoleList = _roleManager.Roles.Select(r => r.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser { 
                    UserName = $"{Input.FirstName.Substring(0,1)}{Input.Lastname}{Input.EmployeeNumber.ToString()}", 
                    Email = Input.Email, 
                    EmployeeNumber = Input.EmployeeNumber,
                    FirstName = Input.FirstName,
                    Lastname = Input.Lastname,
                    Position = Input.Position,
                    PhoneNumber = Input.PhoneNumber    
                };
                try
                {
                    var result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        if(Input.Role == null)
                        {
                            await _userManager.AddToRoleAsync(user, Const.Role_Employee);
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, Input.Role);
                        }

                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        //var callbackUrl = Url.Page(
                        //    "/Account/ConfirmEmail",
                        //    pageHandler: null,
                        //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        //    protocol: Request.Scheme);

                        //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                        

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            if (User.IsInRole(Const.Role_Admin))
                            {

                                _notyf.Success("User created succesfully.");
                                return RedirectToPage("Register");
                            }
                            else
                            {
                                await _signInManager.SignInAsync(user, isPersistent: true);

                                return LocalRedirect(returnUrl);
                            }
                            
                            
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        Input = new InputModel()
                        {
                            RoleList = _roleManager.Roles.Select(r => r.Name).Select(i => new SelectListItem
                            {
                                Text = i,
                                Value = i
                            })
                        };

                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch(DbUpdateException ex)
                {
                    SqlException innerException = ex.InnerException as SqlException;
                    if (innerException != null && innerException.Number == 2601)
                    {
                        ModelState.AddModelError(string.Empty, "Employee number or email already registered, try a different one.");
                        return Page();
                    }
                    else
                    {
                        throw;
                    };
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
