using OnlineCafe.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineCafe.Storage;

namespace OnlineCafe.Services
{
    public interface IOrdersManagementService
    {
        OrdersAdminViewModel GetOrders();
        OrderForPageAdminViewModel GetOrder(int id);
        EditOrderViewModel EditPage(int id);
        Task Edit(int id, EditOrderViewModel model);
    }


    public class OrdersManagementService : IOrdersManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;


        public OrdersManagementService(ApplicationDbContext context, UserManager<User> userManager,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }


        public OrdersAdminViewModel GetOrders()
        {
            var orders = _context.Orders.Select(o => new OrderForPageAdminViewModel
            {
                Id = o.Id
            }).ToList();


            foreach (var order in orders)
            {
                var orderInDB = GetOrder((int)order.Id);
                order.Address = orderInDB.Address;
                order.CreateTime = orderInDB.CreateTime;
                order.StatusOrder = orderInDB.StatusOrder;
                order.UserId = orderInDB.UserId;
                order.DeliveryTime = orderInDB.DeliveryTime;
                order.meals = orderInDB.meals;
            }

            var OrdersAdminViewModel = new OrdersAdminViewModel();
            OrdersAdminViewModel.Orders = orders;

            OrdersAdminViewModel.Orders.Sort((x, y) =>
            {
                int ret = x.StatusOrder.CompareTo(y.StatusOrder);
                return ret != 0 ? ret : y.CreateTime.CompareTo(x.CreateTime);
            });


            return OrdersAdminViewModel;
        }


        public OrderForPageAdminViewModel GetOrder(int id)
        {
            var order = _context.Orders.Include(x => x.User).FirstOrDefault(x => x.Id == id);

            var orderMeal = _context.OrderMeal.Include(x => x.Meal).Where(x => x.Order == order).ToList();


            OrderForPageAdminViewModel result = new OrderForPageAdminViewModel()
            {
                meals = new List<OrdersAdmin>(),
                Address = order.Address,
                Id = order.Id,
                DeliveryTime = order.DeliveryTime,
                CreateTime = order.CreateTime,
                StatusOrder = order.StatusOrder,
                UserId = order.User.Id.ToString()
            };

            foreach (var ord in orderMeal)
            {
                var meal = _context.Meals.Where(x => x.Id == ord.Meal.Id).FirstOrDefault();
                result.meals.Add(new OrdersAdmin
                {
                    Description = meal.Description,
                    CategoryOfMeal = meal.CategoryOfMeal,
                    Id = meal.Id,
                    isVegan = meal.isVegan,
                    Name = meal.Name,
                    Price = meal.Price
                });
            }


            return result;
        }

        public EditOrderViewModel EditPage(int id)
        {
            var order = GetOrder(id);

            List<DateTime> dateTimes = new List<DateTime>();
            var config = _configuration.GetSection("AmountHours");
            int mintoDel = int.Parse(config["MinimumToDelivery"]);
            DateTime timeForCompare = DateTime.Now.AddHours(9);

            for (DateTime i = DateTime.Now; i <= timeForCompare; i = i.AddHours(mintoDel))
            {
                dateTimes.Add(i);
            }

            dateTimes.RemoveAt(0);
            dateTimes.RemoveAt(1);

            EditOrderViewModel viewModel = new EditOrderViewModel();
            viewModel.dateTimes = dateTimes;
            viewModel.orderId = id;
            viewModel.status = order.StatusOrder;

            return viewModel;
        }

        public async Task Edit(int id, EditOrderViewModel model)
        {
            var order = _context.Orders.FirstOrDefault(x => x.Id == id);

            order.DeliveryTime = model.dateTimes[0];
            order.StatusOrder = model.status;


            await _context.SaveChangesAsync();
        }
    }
}