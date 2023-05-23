namespace OnlineCafe.Storage
{
    public class Order
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public DateTime DeliveryTime { get; set; }
        public DateTime CreateTime { get; set; }
        public int PercentDiscount { get; set; }

        public StatusOrder StatusOrder { get; set; }

        public User User { get; set; }

        public IEnumerable<OrderMeal> Ordermeals { get; set; }

        public int Price { get; set; }


    }


    public enum StatusOrder
    {
        New,
        InWork,
        Complete,
        Delivered
    }
}
