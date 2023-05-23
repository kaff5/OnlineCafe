using OnlineCafe.Storage;

namespace OnlineCafe.ViewModels
{
    public class EditOrderViewModel
    {
        public int orderId { get; set; }
        public List<DateTime> dateTimes { get; set; }
        public StatusOrder status { get; set; }



    }
}
