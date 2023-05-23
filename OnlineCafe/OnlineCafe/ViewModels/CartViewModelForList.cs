using OnlineCafe.Storage;

namespace OnlineCafe.ViewModels
{
    public class CartViewModelForList
    {
        public List<MealsViewModel> meals { get; set; }
        public User user { get; set; }

        public DateTime lastUpdate { get; set; }

    }
}
