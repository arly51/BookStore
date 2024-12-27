using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BLL.Controllers.Bases;
using BLL.Services;
using BLL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : MvcController
    {
        private readonly IUserService _userService;
        private readonly IRolesService _roleService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            IRolesService roleService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var loginResult = _userService.Login(model.Username, model.Password);
                if (loginResult.IsSuccessful)
                {
                    var dbUser = _userService.Query()
                        .FirstOrDefault(u => u.Record.UserName.ToUpper() == model.Username.ToUpper().Trim());

                    if (dbUser != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, dbUser.UserName),
                            new Claim(ClaimTypes.Role, dbUser.RoleName),
                            new Claim("UserId", dbUser.Record.Id.ToString())
                        };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            principal,
                            new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTime.UtcNow.AddHours(24)
                            });

                        return RedirectToAction("Index", "Home");
                    }
                }
                ModelState.AddModelError("", "Invalid username or password");
            }
            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Index()
        {
            var users = _userService.Query().ToList();
            return View(users);
        }

        public IActionResult Details(int id)
        {
            var user = _userService.Query().SingleOrDefault(u => u.Record.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        protected void SetViewData()
        {
            ViewData["RoleId"] = new SelectList(_roleService.Query().ToList(), "Record.Id", "Name");
        }

        public IActionResult Create()
        {
            SetViewData();
            return View(new UserModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(user.Password))
                {
                    ModelState.AddModelError("Password", "Password is required");
                    SetViewData();
                    return View(user);
                }

                var result = _userService.Create(user.Record);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(Details), new { id = user.Record.Id });
                }
                ModelState.AddModelError("", result.Message);
            }
            SetViewData();
            return View(user);
        }

        public IActionResult Edit(int id)
        {
            var user = _userService.Query().SingleOrDefault(u => u.Record.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            SetViewData();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var result = _userService.Update(user.Record);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(Details), new { id = user.Record.Id });
                }
                ModelState.AddModelError("", result.Message);
            }
            SetViewData();
            return View(user);
        }

        public IActionResult Delete(int id)
        {
            var user = _userService.Query().SingleOrDefault(u => u.Record.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var result = _userService.Delete(id);
            TempData["Message"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (int.TryParse(User.FindFirst("UserId")?.Value, out int userId))
                {
                    var result = _userService.ChangePassword(userId, model.CurrentPassword, model.NewPassword);
                    if (result.IsSuccessful)
                    {
                        TempData["Message"] = "Password changed successfully";
                        return RedirectToAction(nameof(Index), "Home");
                    }
                    ModelState.AddModelError("", result.Message);
                }
                else
                {
                    ModelState.AddModelError("", "User ID not found");
                }
            }
            return View(model);
        }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}