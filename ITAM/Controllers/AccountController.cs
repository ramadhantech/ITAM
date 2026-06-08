using System.Security.Claims;
using ITAM.Dto;
using ITAM.Services; // Panggil namespace Service Anda
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace ITAM.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService; // 1. Suntikkan UserService di sini

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto request)
        {
            if (!ModelState.IsValid) return View(request);

            // 2. KUNCI UTAMA: Validasi data login diserahkan sepenuhnya ke lapisan Service
            var user = await _userService.ValidateLoginAsync(request.Email, request.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email atau password yang Anda masukkan salah.");
                return View(request);
            }

            // 3. Jika user ditemukan, Controller menerbitkan tiket sesi masuk (Cookie)
            var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Name, user.Name),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Role, user.Role),

    // FIX INI
    new Claim("Department", user.Department?.Name ?? "")
};

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // Redireksi halaman berdasarkan hak akses
            if (user.Role.ToUpper() == "ADMIN")
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Approval");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
