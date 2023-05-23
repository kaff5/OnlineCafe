using System.ComponentModel.DataAnnotations;

namespace OnlineCafe.ViewModels
{
    public class EditAccountProfileViewModel
    {

        public int Id { get; set; }
        [Display(Name = "Дата рождения")]
        [Required(ErrorMessage = "Дата рождения обязательна для заполнения")]
        public DateTime BirthDate { get; set; }


        [Required(ErrorMessage = "ФИО обязательно для заполнения")]
        [Display(Name = "ФИО")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Телефон обязателен для заполнения")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

    }
}
