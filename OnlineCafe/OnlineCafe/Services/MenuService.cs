using Microsoft.AspNetCore.Identity;
using OnlineCafe.Storage;
using OnlineCafe.ViewModels;

namespace OnlineCafe.Services
{
    public interface IMenuService
    {
        Task<MenuViewModel> GetMeals();
        Task<MenuViewModel> GetMeals(CategoryOfMeal[] category);
        Task<MenuViewModel> GetMeals(bool isVegan);
        Task<MenuViewModel> GetMeals(CategoryOfMeal[] category, bool isVegan);
        Task<MealsViewModel> GetMeal(int id);
        Task Edit(int id, MealsViewModel model);
        Task Delete(int id);
        Task Create(MealsViewModel model);
    }


    public class MenuService : IMenuService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private static string[] AllowedExtensions { get; set; } = { "jpg", "jpeg", "png" };

        public MenuService(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        public async Task<MenuViewModel> GetMeals()
        {
            MenuViewModel menu = new MenuViewModel();
            menu.Meals = new List<MealsViewModel>();
            menu.Meals = _context.Meals.Where(x => x.isDelete == false).Select(x => new MealsViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                Description = x.Description,
                isVegan = x.isVegan,
                CategoryOfMeal = x.CategoryOfMeal,
                path = x.path,
            }).ToList();
            return menu;
        }

        public async Task<MenuViewModel> GetMeals(CategoryOfMeal[] category)
        {
            var meals = _context.Meals.Where(x => category.Contains(x.CategoryOfMeal) && x.isDelete == false).ToList();
            MenuViewModel menu = new MenuViewModel();
            menu.Meals = new List<MealsViewModel>();
            foreach (var meal in meals)
            {
                menu.Meals.Add(new MealsViewModel
                {
                    Id = meal.Id,
                    Description = meal.Description,
                    Price = meal.Price,
                    Name = meal.Name,
                    isVegan = meal.isVegan,
                    CategoryOfMeal = meal.CategoryOfMeal,
                    path = meal.path,
                });
            }

            return menu;
        }

        public async Task<MenuViewModel> GetMeals(CategoryOfMeal[] category, bool isVegan)
        {
            var meals = _context.Meals
                .Where(x => category.Contains(x.CategoryOfMeal) && x.isVegan == isVegan && x.isDelete == false)
                .ToList();
            MenuViewModel menu = new MenuViewModel();
            menu.Meals = new List<MealsViewModel>();
            foreach (var meal in meals)
            {
                menu.Meals.Add(new MealsViewModel
                {
                    Id = meal.Id,
                    Description = meal.Description,
                    Price = meal.Price,
                    Name = meal.Name,
                    isVegan = meal.isVegan,
                    CategoryOfMeal = meal.CategoryOfMeal,
                    path = meal.path,
                });
            }

            return menu;
        }


        public async Task<MenuViewModel> GetMeals(bool isVegan)
        {
            var meals = _context.Meals.Where(x => x.isVegan == isVegan && x.isDelete == false).ToList();
            MenuViewModel menu = new MenuViewModel();
            menu.Meals = new List<MealsViewModel>();
            foreach (var meal in meals)
            {
                menu.Meals.Add(new MealsViewModel
                {
                    Id = meal.Id,
                    Description = meal.Description,
                    Price = meal.Price,
                    Name = meal.Name,
                    isVegan = meal.isVegan,
                    CategoryOfMeal = meal.CategoryOfMeal,
                    path = meal.path,
                });
            }

            return menu;
        }


        public async Task<MealsViewModel> GetMeal(int id)
        {
            Meal meal = await _context.Meals.FindAsync(id);


            MealsViewModel mealViewModel = new MealsViewModel
            {
                Name = meal.Name,
                Price = meal.Price,
                Description = meal.Description,
                isVegan = meal.isVegan,
                CategoryOfMeal = meal.CategoryOfMeal,
                Id = id,
                path = meal.path,
            };
            return mealViewModel;
        }


        public async Task Edit(int id, MealsViewModel model)
        {
            Meal meal = _context.Meals.Find(id);
            if (System.IO.File.Exists($"wwwroot{meal.path}"))
            {
                System.IO.File.Delete($"wwwroot{meal.path}");
            }

            var isFileAttached = model.File != null;
            var fileNameWithPath = string.Empty;
            if (isFileAttached)
            {
                var extension = Path.GetExtension(model.File.FileName).Replace(".", "");
                if (!AllowedExtensions.Contains(extension))
                {
                    throw new ArgumentException("Attached file has not supported extension");
                }

                fileNameWithPath = $"Files/{Guid.NewGuid()}-{model.File.FileName}";
                using (var fs = new FileStream(Path.Combine(_environment.WebRootPath, fileNameWithPath),
                           FileMode.Create))
                {
                    await model.File.CopyToAsync(fs);
                }
            }


            meal.Name = model.Name;
            meal.Price = model.Price;
            meal.Description = model.Description;
            meal.isVegan = model.isVegan;
            meal.CategoryOfMeal = model.CategoryOfMeal;
            meal.path = fileNameWithPath;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            Meal meal = _context.Meals.Find(id);
            if (System.IO.File.Exists($"wwwroot/{meal.path}"))
            {
                System.IO.File.Delete($"wwwroot/{meal.path}");
            }


            _context.Meals.Remove(meal);

            await _context.SaveChangesAsync();
        }

        public async Task Create(MealsViewModel model)
        {
            Meal meal = _context.Meals.FirstOrDefault(x => x.Name == model.Name);
            if (meal != null)
            {
                throw new ArgumentException($"Блюдо с таким название уже существует");
            }

            var isFileAttached = model.File != null;
            var fileNameWithPath = string.Empty;
            if (isFileAttached)
            {
                var extension = Path.GetExtension(model.File.FileName).Replace(".", "");
                if (!AllowedExtensions.Contains(extension))
                {
                    throw new ArgumentException("Attached file has not supported extension");
                }

                fileNameWithPath = $"Files/{Guid.NewGuid()}-{model.File.FileName}";
                using (var fs = new FileStream(Path.Combine(_environment.WebRootPath, fileNameWithPath),
                           FileMode.Create))
                {
                    await model.File.CopyToAsync(fs);
                }
            }

            await _context.Meals.AddAsync(new Meal
            {
                Name = model.Name,
                Price = model.Price,
                Description = model.Description,
                isVegan = model.isVegan,
                CategoryOfMeal = model.CategoryOfMeal,
                isDelete = false,
                path = fileNameWithPath,
            });
            await _context.SaveChangesAsync();
        }
    }
}