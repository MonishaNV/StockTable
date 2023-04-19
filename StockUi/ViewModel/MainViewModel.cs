using StockUi.DataBaseOperation;
using StockUi.Model;
using StockUi.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace StockUi.ViewModel
{

    public class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly ObservableCollection<Stock> _stocks;
        private readonly IStockLiveUpdateService _stockLiveUpdateService;
        private DatabaseMaintainOperations _databaseViewmodel;

        private Color _raisingStockColor = Colors.Black;
        private Color _fallingStockColor = Colors.Black;

        public MainViewModel(IStockLiveUpdateService stockLiveUpdateService)
        {
            _stocks = new ObservableCollection<Stock>();
            _stockLiveUpdateService = stockLiveUpdateService;
            _stockLiveUpdateService.StockUpdated += OnStockUpdated;
            _databaseViewmodel = new DatabaseMaintainOperations(_stockLiveUpdateService);

            _stockLiveUpdateService.ConnectToSignalRHub();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnStockUpdated(Stock stock)
        {
            var existingStock = Stocks.FirstOrDefault(s => s.Symbol == stock.Symbol);
            if (existingStock != null)
            {
                int index = Stocks.IndexOf(existingStock);
                Stocks.RemoveAt(index);

                existingStock.Time = DateTime.Now.ToString("h:mm:ss tt");
                existingStock.Price = stock.Price;
                existingStock.Change = stock.Change;
                existingStock.PercentChange = Math.Round(100 - (existingStock.Price - existingStock.Change) / existingStock.Price * 100, 6);

                Stocks.Insert(index, existingStock);
            }
            else
            {
                Stocks.Add(new Stock(stock.Symbol, stock.Price, stock.Change));
            }
            OnPropertyChanged(nameof(Stock));
        }


        public ObservableCollection<Stock> Stocks => _stocks;

        public Color RaisingStockColor
        {
            get { return _raisingStockColor; }
            set
            {
                if (_raisingStockColor != value)
                {
                    _raisingStockColor = value;
                    OnPropertyChanged(nameof(RaisingStockColor));
                    OnPropertyChanged(nameof(RaisingStockBrush));
                }
            }
        }

        public Brush RaisingStockBrush => new SolidColorBrush(RaisingStockColor);

        public Color FallingStockColor
        {
            get { return _fallingStockColor; }
            set
            {
                if (_fallingStockColor != value)
                {
                    _fallingStockColor = value;
                    OnPropertyChanged(nameof(FallingStockColor));
                    OnPropertyChanged(nameof(FallingStockBrush));
                }
            }
        }

        public Brush FallingStockBrush => new SolidColorBrush(FallingStockColor);

        public void Dispose()
        {
            _stockLiveUpdateService.StockUpdated -= OnStockUpdated;
            _databaseViewmodel.Dispose();
        }

    }
}