

namespace OnlineCafe.Storage
{
    public class Address
    {
        public User User { get; set; }
        public int Id { get; set; }
        public string Street { get; set; }
        public string House { get; set; }

        public string Entrance { get; set; }
        public string Flat { get; set; }
        public string Note { get; set; }
        public string Name { get; set; }
        public bool MainAddress { get; set; }
    }
}
