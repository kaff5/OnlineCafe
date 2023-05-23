using OnlineCafe.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using OnlineCafe.Services;

namespace OnlineCafe.Controllers
{
    public class AccountProfileController : Controller
    {
        private readonly IUsersService _usersService;

        public AccountProfileController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var model = await _usersService.GetInfo(
                Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value));
            return View(model);
        }


        [HttpGet]
        public IActionResult AddAddress()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _usersService.AddAddress(
                        Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value), model);
                    return RedirectToAction("Index", "AccountProfile");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("RegistrationErrors", ex.Message);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditAddressPage(int Id)
        {
            var model = await _usersService.GetAddress(
                Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value), Id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [Route("AccountProfile/EditAddressPage/{id}")]
        public async Task<IActionResult> EditAddressPage(AddressViewModel model, int Id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _usersService.EditAddress(
                        Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value), Id,
                        model);
                    return RedirectToAction("Index", "AccountProfile");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("RegistrationErrors", ex.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteAddress(int Id)
        {
            try
            {
                await _usersService.DeleteAddress(
                    Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value), Id);
                return RedirectToAction("Index", "AccountProfile");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("RegistrationErrors", ex.Message);
            }

            return null;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmDeleteAddress(int id)
        {
            var model = await _usersService.GetAddress(
                Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value), id);
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditAccountProfile(AccountProfileViewModel model)
        {
            var model2 = await _usersService.GetInfoForEdit(
                Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value), model);
            return View(model2);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [Route("AccountProfile/EditAccountProfile/{id}")]
        public async Task<IActionResult> EditAccountProfile(int Id, EditAccountProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _usersService.EditProfile(
                        Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value), Id,
                        model);
                    return RedirectToAction("Index", "AccountProfile");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("RegistrationErrors", ex.Message);
                }
            }

            return View(model);
        }
    }
}