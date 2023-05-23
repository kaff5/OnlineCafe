namespace OnlineCafe.Services
{
    public class CartCleaner : IHostedService
    {
        private Timer _timer = null!;
        private readonly CleanerConfig _cleanerConfig;
        private readonly ICartService _cart;

        public CartCleaner(ICartService cart, CleanerConfig cleanerConfig)
        {
            _cart = cart;
            _cleanerConfig = cleanerConfig;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(20));

            return Task.CompletedTask;
        }


        public void DoWork(object? state)
        {
            _cart.GetLastUpdatesAndDelete(_cleanerConfig.Time);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);


            return Task.CompletedTask;
        }
    }
}