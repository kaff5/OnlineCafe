using Microsoft.AspNetCore.Mvc;
using OnlineCafe.Services;
using OnlineCafe.ViewModels;

namespace OnlineCafe.Controllers
{
	public class AccountController : Controller
    {
        private readonly IUsersService _usersService;

        public AccountController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _usersService.Register(model);
                    return RedirectToAction("Index", "Menu");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("RegistrationErrors", ex.Message);
                }
            }

            return
                View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _usersService.Login(model);
                    return RedirectToAction("Index", "Menu");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Errors", ex.Message);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _usersService.Logout();
            return RedirectToAction("Index", "Menu");
        }
    }
}