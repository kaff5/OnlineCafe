using OnlineCafe.Storage;

namespace OnlineCafe.ViewModels
{
    public class MenuViewModel
    {
        public List<MealsViewModel> Meals { get; set; }
        public bool? isVegan { get; set; }
        public CategoryOfMeal[]? Categories { get; set; }
    }
}
