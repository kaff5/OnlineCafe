using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace OnlineCafe.ViewModels
{
    public class AddressViewModel
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Адрес обязателен для заполнения")]
        [Display(Name = "Улица")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Адрес обязателен для заполнения")]
        [Display(Name = "Дом")]
        public string House { get; set; }

        [Required(ErrorMessage = "Адрес обязателен для заполнения")]
        [Display(Name = "Подъезд")]
        public string Entrance { get; set; }


        [Required(ErrorMessage = "Адрес обязателен для заполнения")]
        [Display(Name = "Квартира")]
        public string Flat { get; set; }

        [Required(ErrorMessage = "Адрес обязателен для заполнения")]
        [Display(Name = "Примечание")]
        public string Note { get; set; }


        [Required(ErrorMessage = "Адрес обязателен для заполнения")]
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Адрес обязателен для заполнения")]
        [Display(Name = "Сделать главным адресом")]
        public bool MainAddress { get; set; }

    }
}
