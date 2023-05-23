using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using OnlineCafe.Services;

namespace OnlineCafe.Controllers
{
    public class CartController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly ICartService _cartService;

        public CartController(IMenuService menuService, ICartService cartService)
        {
            _menuService = menuService;
            _cartService = cartService;
        }


        public async Task<IActionResult> Index()
        {
            var model = _cartService.GetCart(
                Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value));
            return View(model);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddToCart(int id)
        {
            var model = await _menuService.GetMeal(id);
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitAddToCart(int id)
        {
            _cartService.AddToCard(
                Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value), id);
            return RedirectToAction("Index", "Menu");
        }

        [HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitDelete(int id)
        {
            _cartService.Delete(Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value),
                id);
            return RedirectToAction("Index", "Cart");
        }

        [HttpGet]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var model = _cartService.GetCart(
                Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value), id);
            return View(model);
        }
    }
}