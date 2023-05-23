using OnlineCafe.ViewModels;
using OnlineCafe.Storage;

namespace OnlineCafe.Services
{
    public interface ICartService
    {
        void AddToCard(Guid userId, int id);
        CardDishesViewModels GetCart(Guid userId);
        CardDishesViewModels Delete(Guid UserId, int id);
        MealsViewModel GetCart(Guid UserId, int id);
        void GetLastUpdatesAndDelete(int _configTime);

        void RemoveCartUser(Guid userId);
    }

    public class CartService : ICartService
    {
        private List<CartViewModelForList> _cart { get; set; } = new List<CartViewModelForList>();


        private readonly IServiceScopeFactory _scopeFactory;

        public CartService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }


        public void AddToCard(Guid userId, int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


                var meal = db.Meals.FirstOrDefault(x => x.Id == id);

                var user = db.Users.FirstOrDefault(x => x.Id == userId);

                if (meal == null)
                {
                    throw new KeyNotFoundException("Этого блюда больше не существует");
                }

                lock (_cart)
                {
                    var UserCart = _cart.FirstOrDefault(x => x.user.Id == user.Id);
                    if (UserCart == null)
                    {
                        _cart.Add(new CartViewModelForList
                        {
                            meals = new List<MealsViewModel>(),
                            user = user,
                            lastUpdate = DateTime.Now
                        });
                    }

                    UserCart = _cart.FirstOrDefault(x => x.user.Id == user.Id);
                    UserCart.lastUpdate = DateTime.Now;
                    var UserCartMeal = UserCart.meals.FirstOrDefault(x => x.Id == id);
                    if (UserCartMeal == null)
                    {
                        UserCart.meals.Add(new MealsViewModel
                        {
                            Id = meal.Id,
                            Description = meal.Description,
                            CategoryOfMeal = meal.CategoryOfMeal,
                            Count = 1,
                            isVegan = meal.isVegan,
                            Name = meal.Name,
                            Price = meal.Price,
                        });
                    }
                    else
                    {
                        UserCartMeal.Count += 1;
                    }
                }
            }
        }

        public CardDishesViewModels GetCart(Guid userId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var user = db.Users.FirstOrDefault(x => x.Id == userId);

                CardDishesViewModels model = new CardDishesViewModels();
                model.meals = new List<MealsViewModel>();

                lock (_cart)
                {
                    var UserCart = _cart.FirstOrDefault(x => x.user.Id == user.Id);
                    if (UserCart == null)
                    {
                        _cart.Add(new CartViewModelForList
                        {
                            meals = new List<MealsViewModel>(),
                            user = user,
                            lastUpdate = DateTime.Now
                        });
                    }

                    UserCart = _cart.FirstOrDefault(x => x.user.Id == user.Id);
                    UserCart.lastUpdate = DateTime.Now;

                    foreach (var meal in UserCart.meals)
                    {
                        model.meals = UserCart.meals;
                    }
                }


                return model;
            }
        }


        public CardDishesViewModels Delete(Guid UserId, int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var user = db.Users.FirstOrDefault(x => x.Id == UserId);


                lock (_cart)
                {
                    var UserCart = _cart.FirstOrDefault(x => x.user.Id == user.Id);
                    UserCart.lastUpdate = DateTime.Now;

                    var mealCartUser = UserCart.meals.FirstOrDefault(x => x.Id == id);

                    if (mealCartUser.Count > 1)
                    {
                        mealCartUser.Count -= 1;
                    }
                    else
                    {
                        UserCart.meals.Remove(mealCartUser);
                    }
                }


                return GetCart(UserId);
            }
        }


        public MealsViewModel GetCart(Guid UserId, int id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                MealsViewModel mealCart = new MealsViewModel();


                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var user = db.Users.FirstOrDefault(x => x.Id == UserId);


                var UserCart = _cart.FirstOrDefault(x => x.user.Id == user.Id);

                var mealCartUser = UserCart.meals.FirstOrDefault(x => x.Id == id);


                return mealCartUser;
            }
        }


        public void GetLastUpdatesAndDelete(int _configTime)
        {
            var carts = _cart.Where(x => (DateTime.Now - x.lastUpdate).TotalMinutes >= _configTime).ToList();


            foreach (var cart in carts)
            {
                _cart.Remove(cart);
            }
        }


        public void RemoveCartUser(Guid userId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var user = db.Users.FirstOrDefault(x => x.Id == userId);


                var UserCart = _cart.FirstOrDefault(x => x.user.Id == user.Id);


                UserCart.meals = new List<MealsViewModel>();
            }
        }
    }
}