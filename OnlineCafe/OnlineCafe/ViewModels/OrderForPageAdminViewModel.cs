using OnlineCafe.Storage;

namespace OnlineCafe.ViewModels
{
    public class OrderForPageAdminViewModel
    {
        public int? Id { get; set; }
        public string Address { get; set; }
        public DateTime DeliveryTime { get; set; }
        public DateTime CreateTime { get; set; }

        public StatusOrder StatusOrder { get; set; }

        public string UserId { get; set; }

        public List<OrdersAdmin> meals { get; set; }
    }
}
