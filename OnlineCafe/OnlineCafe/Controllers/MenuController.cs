using OnlineCafe.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineCafe.Services;
using OnlineCafe.Storage;

namespace OnlineCafe.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }


        public async Task<IActionResult> Index(CategoryOfMeal[]? category, bool? isVegan)
        {
            if (!category.Any() && isVegan == null)
            {
                var model = await _menuService.GetMeals();
                return View(model);
            }
            else if (!category.Any() && isVegan != null)
            {
                var model = await _menuService.GetMeals(true);
                return View(model);
            }
            else if (category.Any() && isVegan == null)
            {
                var model = await _menuService.GetMeals(category);
                return View(model);
            }
            else if (category.Any() && isVegan != null)
            {
                var model = await _menuService.GetMeals(category, true);
                return View(model);
            }

            return View();
        }


        [HttpGet]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MealsViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _menuService.Create(model);
                    return RedirectToAction("Index", "Menu");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("RegistrationErrors", ex.Message);
                }
            }

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        public async Task<IActionResult> Update(int id)
        {
            var model = await _menuService.GetMeal(id);
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, MealsViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _menuService.Edit(id, model);
                    return RedirectToAction("Index", "Menu");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("RegistrationErrors", ex.Message);
                }
            }

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        public async Task<IActionResult> Delete(int id)
        {
            var model = await _menuService.GetMeal(id);
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = ApplicationRoleNames.Administrator)]
        public async Task<IActionResult> SubmitDelete(int id)
        {
            await _menuService.Delete(id);
            return RedirectToAction("Index", "Menu");
        }
    }
}