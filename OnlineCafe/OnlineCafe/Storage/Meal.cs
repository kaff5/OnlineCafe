namespace OnlineCafe.Storage
{
    public class Meal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public bool isVegan { get; set; }
        public CategoryOfMeal CategoryOfMeal { get; set; }

        public bool isDelete { get; set; }
        public string? path { get; set; }

        public IEnumerable<OrderMeal> Ordermeals { get; set; }



    }


    public enum CategoryOfMeal
    {
        WOK,
        Pizza,
        Soup,
        Dessert,
        Drink
    }
}
