using System;
namespace StockUi.Model
{
    public class Stock
    {
        public Stock(string symbol, decimal price, decimal change)
        {
            Time = DateTime.Now.ToString("h:mm:ss tt");
            Symbol = symbol;
            Price = price;
            Change = change;
            PercentChange = Math.Round((100 - ((Price - Change) / Price) * 100), 6);
        }

        public string Time { get; set; }
        
        public string Symbol { get; set; }

        public decimal Price { get; set; }

        public decimal Change { get; set; }

        public decimal PercentChange { get; set; }
    }
}
