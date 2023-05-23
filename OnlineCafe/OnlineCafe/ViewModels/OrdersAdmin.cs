using System.ComponentModel.DataAnnotations;
using OnlineCafe.Storage;

namespace OnlineCafe.ViewModels
{
    public class OrdersAdmin
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя обязательно для заполнения")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Цена обязятельно для заполнения")]
        [Display(Name = "Цена")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Описание обязятельно для заполнения")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        public bool isVegan { get; set; }
        public CategoryOfMeal CategoryOfMeal { get; set; }

        public int? Count { get; set; }

        public string? path { get; set; }


    }
}
