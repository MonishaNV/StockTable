using Microsoft.AspNetCore.SignalR.Client;
using StockUi.Model;


namespace StockUi.Services
{
    public class StockLiveUpdateService : IStockLiveUpdateService
    {
        private HubConnection _hubConnection;

        public async void ConnectToSignalRHub()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://js.devexpress.com/Demos/NetCore/liveUpdateSignalRHub")
                .Build();

            _hubConnection.On<Stock>("updateStockPrice", stock =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    StockUpdated?.Invoke(stock);
                });
            });

            await _hubConnection.StartAsync();
        }
        public event StockUpdatedDelegate StockUpdated;

    }
    public interface IStockLiveUpdateService
    {
        event StockUpdatedDelegate StockUpdated;
        void ConnectToSignalRHub();
    }

    public delegate void StockUpdatedDelegate(Stock stock);
}
