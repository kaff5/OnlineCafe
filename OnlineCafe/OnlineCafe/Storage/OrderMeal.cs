namespace OnlineCafe.Storage
{
    public class OrderMeal
    {
        public int Id { get; set; }
        public Order Order { get; set; }
        public Meal Meal { get; set; }
        public int Count { get; set; }
    }
}
