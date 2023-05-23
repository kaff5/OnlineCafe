using OnlineCafe.Storage;

namespace OnlineCafe.ViewModels
{
    public class OrderCreateViewModel
    {
        public int Price { get; set; }
        public CardDishesViewModels CardDishesViewModels { get; set; }

        public List<string> Addresess { get; set; }
        public IEnumerable<AddressViewModel> AddresessEnum { get; set; }


        public List<DateTime> TimeToDelivery { get; set; }

        public int Discount { get; set; }

        public int? PriceWithDiscount { get; set; }

        public Address? ChooseAddress { get; set; }


    }


}
