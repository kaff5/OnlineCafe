using System.ComponentModel.DataAnnotations;

namespace OnlineCafe.ViewModels
{
    public class AccountProfileViewModel
    {

        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }

        public DateTime? BirthDate { get; set; }
        public string? Phone { get; set; }

        public List<AddressViewModel>? Addresses { get; set; }

    }
}
