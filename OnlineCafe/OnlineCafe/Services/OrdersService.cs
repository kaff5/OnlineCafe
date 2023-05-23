using OnlineCafe.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using OnlineCafe.Storage;

namespace OnlineCafe.Services
{
    public interface IOrdersService
    {
        Task<OrderCreateViewModel> GetOrderPage(Guid userId);
        Task<OrdersViewModel> GetOrdersAndCart(Guid userId);
        Task ToOrder(Guid user, OrderCreateViewModel model);
    }


    public class OrdersService : IOrdersService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ICartService _cart;
        private readonly IConfiguration _configuration;


        public OrdersService(ApplicationDbContext context, UserManager<User> userManager, ICartService cart,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _cart = cart;
            _configuration = configuration;
        }


        public async Task<OrderCreateViewModel> GetOrderPage(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User does not found");
            }

            List<Address> address = _context.UserAddress.Where(x => x.User == user).ToList();
            if (address == null)
            {
                throw new KeyNotFoundException($"Адреса не найдены");
            }

            DateTime minusThree = DateTime.Now.AddDays(-3);
            DateTime plusThree = DateTime.Now.AddDays(+3);
            DateTime nowTime = DateTime.Now;

            DateTime dateHoursMinusThree = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 12, 0, 0);
            DateTime dateHoursPlusThree = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, 15, 0, 0);
            int Discount = 0;

            if (user.BirthDate.Month == minusThree.Month)
            {
                if (user.BirthDate.Day >= minusThree.Day && user.BirthDate.Day <= plusThree.Day)
                {
                    Discount = 15;
                }
            }
            else if (nowTime >= dateHoursMinusThree && nowTime <= dateHoursPlusThree)
            {
                Discount = 10;
            }

            CardDishesViewModels carts = _cart.GetCart(userId);
            int Price = 0;
            var PriceWithDiscount = 0;
            foreach (var meal in carts.meals)
            {
                Price += meal.Price;
            }

            if (Discount != 0)
            {
                PriceWithDiscount = Price - ((Price * Discount) / 100);
            }

            List<string> addressess = new List<string>();
            foreach (var addres in address)
            {
                addressess.Add($"{addres.Street},{addres.Flat}");
            }

            List<DateTime> dateTimes = new List<DateTime>();
            var config = _configuration.GetSection("AmountHours");
            int mintoDel = int.Parse(config["MinimumToDelivery"]);
            DateTime timeForCompare = DateTime.Now.AddHours(5);

            for (DateTime i = DateTime.Now; i <= timeForCompare; i = i.AddHours(mintoDel))
            {
                dateTimes.Add(i);
            }

            dateTimes.RemoveAt(0);


            return new OrderCreateViewModel
            {
                Addresess = addressess,
                Price = Price,
                CardDishesViewModels = carts,
                PriceWithDiscount = PriceWithDiscount,
                TimeToDelivery = dateTimes,
                Discount = (int)Discount
            };
        }

        public async Task<OrdersViewModel> GetOrdersAndCart(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User does not found");
            }

            List<Order> orders = _context.Orders.Where(x => x.User == user).ToList();


            OrdersViewModel ordersModel = new OrdersViewModel();
            ordersModel.CardDishesViewModels = _cart.GetCart(userId);
            ordersModel.OrderForPageViewModel = new List<OrderForPageViewModel>();


            foreach (var order in orders)
            {
                List<OrderMeal> OrderMeal =
                    _context.OrderMeal.Include(x => x.Meal).Where(x => x.Order == order).ToList();
                List<Meal> mealsinOrder = new List<Meal>();

                foreach (var meal in OrderMeal)
                {
                    mealsinOrder.Add(_context.Meals.Where(x => x.Id == meal.Meal.Id).FirstOrDefault());
                }

                List<MealsViewModel> MealsModel = new List<MealsViewModel>();

                foreach (var meal in mealsinOrder)
                {
                    MealsModel.Add(new MealsViewModel(meal));
                }


                ordersModel.OrderForPageViewModel.Add(new OrderForPageViewModel
                {
                    meals = MealsModel,
                    DeliveryTime = order.DeliveryTime,
                    CreateTime = order.CreateTime,
                    Address = order.Address,
                    StatusOrder = order.StatusOrder,
                    Id = order.Id,
                });
            }

            return ordersModel;
        }


        public async Task ToOrder(Guid userId, OrderCreateViewModel model)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException($"User does not found");
            }


            /*        public int Id { get; set; }
                    public Order Order { get; set; }
                    public Meal Meal { get; set; }
                    public int Count { get; set; }*/


            var orderMeal = new List<OrderMeal>();
            Order order = new Order
            {
                User = user,
                Address = model.Addresess[0],
                CreateTime = DateTime.Now,
                DeliveryTime = model.TimeToDelivery[0],
                PercentDiscount = model.Discount,
                StatusOrder = StatusOrder.New,
                Price = model.Price,
                Ordermeals = orderMeal
            };


            var cart = _cart.GetCart(userId);

            foreach (var meal in cart.meals)
            {
                var mealInDB = _context.Meals.FirstOrDefault(x => x.Id == meal.Id);

                orderMeal.Add(new OrderMeal
                {
                    Count = (int)meal.Count,
                    Order = order,
                    Meal = mealInDB
                });
            }


            _cart.RemoveCartUser(userId);


            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
    }
}