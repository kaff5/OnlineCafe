using OnlineCafe.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCafe.Services;
using OnlineCafe.Storage;

namespace OnlineCafe.Controllers
{
    public class OrdersManagementController : Controller
    {
        private readonly IOrdersManagementService _ordersManagementService;

        public OrdersManagementController(IOrdersManagementService ordersManagementService)
        {
            _ordersManagementService = ordersManagementService;
        }


        [HttpGet]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        public IActionResult Index()
        {
            var model = _ordersManagementService.GetOrders();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        public IActionResult Details(int id)
        {
            var model = _ordersManagementService.GetOrder(id);
            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        public IActionResult Edit(int id)
        {
            var model = _ordersManagementService.EditPage(id);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        public async Task<IActionResult> Edit(int id, EditOrderViewModel model)
        {
            await _ordersManagementService.Edit(id, model);
            return RedirectToAction($"Details", "OrdersManagement", new { id = id });
        }
    }
}