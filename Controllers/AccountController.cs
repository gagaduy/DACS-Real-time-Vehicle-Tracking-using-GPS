using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DACS.Data;
using DACS.Models;
using DACS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using DACS.Services;

namespace DACS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AccountController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Accounts
                    .Include(a => a.Role)
                    .FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim("FullName", user.FullName),
                        new Claim(ClaimTypes.Role, user.Role.Name),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không chính xác.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login");

            var user = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return NotFound();

            var viewModel = new ProfileViewModel
            {
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                RoleName = user.Role.Name
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return RedirectToAction("Login");

            var user = await _context.Accounts.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return NotFound();

            var viewModel = new EditProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity?.Name;
                var user = await _context.Accounts.FirstOrDefaultAsync(u => u.Username == username);

                if (user != null)
                {
                    user.FullName = model.FullName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Address = model.Address;

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công!";
                    return RedirectToAction("Profile");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity?.Name;
                var user = await _context.Accounts.FirstOrDefaultAsync(u => u.Username == username);

                if (user != null)
                {
                    // Verify current password
                    if (user.Password != model.CurrentPassword)
                    {
                        ModelState.AddModelError("CurrentPassword", "Mật khẩu hiện tại không chính xác.");
                        return View(model);
                    }

                    user.Password = model.NewPassword; // In real app, hash this!
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("Profile");
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Accounts.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null)
                {
                    // Generate Token
                    var token = Guid.NewGuid().ToString();
                    user.PasswordResetToken = token;
                    user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(30);

                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    // Send Email
                    var resetLink = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, Request.Scheme);
                    await _emailService.SendEmailAsync(model.Email, "Đặt lại mật khẩu - GPS Management",
                        $"Chào {user.FullName},<br/><br/>Vui lòng click vào link sau để đặt lại mật khẩu của bạn (có hiệu lực trong 30 phút):<br/><a href='{resetLink}'>{resetLink}</a>");
                }
                
                // Show success message regardless of existence to prevent email enumeration
                TempData["SuccessMessage"] = "Nếu Email tồn tại, hướng dẫn đặt lại mật khẩu đã được gửi đi.";
                return View();
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email)) return RedirectToAction("Login");
            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Accounts.FirstOrDefaultAsync(u => 
                    u.Email == model.Email && u.PasswordResetToken == model.Token);

                if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
                {
                    ModelState.AddModelError(string.Empty, "Mã xác nhận không hợp lệ hoặc đã hết hạn.");
                    return View(model);
                }

                user.Password = model.NewPassword;
                user.PasswordResetToken = null;
                user.ResetTokenExpires = null;

                _context.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Mật khẩu đã được thay đổi thành công. Vui lòng đăng nhập lại.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var username = User.Identity?.Name;
            var user = await _context.Accounts
                .Include(a => a.UserSetting)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return RedirectToAction("Login");

            // Initialize settings if not exists
            if (user.UserSetting == null)
            {
                user.UserSetting = new UserSetting { AccountId = user.Id };
                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            var model = new SettingsViewModel
            {
                SpeedLimitThreshold = user.UserSetting.SpeedLimitThreshold,
                IsNotificationEnabled = user.UserSetting.IsNotificationEnabled
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Settings(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity?.Name;
                var user = await _context.Accounts
                    .Include(a => a.UserSetting)
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user != null && user.UserSetting != null)
                {
                    user.UserSetting.SpeedLimitThreshold = model.SpeedLimitThreshold;
                    user.UserSetting.IsNotificationEnabled = model.IsNotificationEnabled;

                    _context.Update(user.UserSetting);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Cài đặt đã được lưu thành công.";
                    return RedirectToAction("Settings");
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ClearGpsHistory()
        {
            // Xóa sạch bảng GPSHistories
            _context.GPSHistories.RemoveRange(_context.GPSHistories);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xóa toàn bộ lịch sử GPS thành công.";
            return RedirectToAction("Settings");
        }
    }
}
