using OnlineCafe.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using OnlineCafe.Services;

namespace OnlineCafe.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrdersService _ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var model = await _ordersService.GetOrdersAndCart(
                Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value));
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var model = await _ordersService.GetOrderPage(
                Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value));
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            await _ordersService.ToOrder(
                Guid.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value), model);
            return RedirectToAction("Index", "Menu");
        }
    }
}