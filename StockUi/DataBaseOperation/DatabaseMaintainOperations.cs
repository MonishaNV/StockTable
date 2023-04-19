using StockUi.Model;
using StockUi.Services;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace StockUi.DataBaseOperation
{
    internal class DatabaseMaintainOperations : IDisposable
    {
        private IStockLiveUpdateService _stockLiveUpdateService;
        private const int MAX_OLD_ENTRIES_POSIBBLE = 9;
        private SqlConnection _sqlConnection;
        string _connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\akshay\\source\\repos\\StockTable-Monisha\\StockUi\\Database\\db_stocks.mdf;Integrated Security=True";

        public DatabaseMaintainOperations(IStockLiveUpdateService stockLiveUpdateService)
        {
            _stockLiveUpdateService = stockLiveUpdateService;
            _stockLiveUpdateService.StockUpdated += UpdateDatabase;
            _sqlConnection = new SqlConnection(_connectionString);
        }

        private void UpdateDatabase(Stock stock)
        {
            try
            {
                if (_sqlConnection.State != ConnectionState.Open) { _sqlConnection.Open(); }
                SqlDataReader reader = GetOldEntries(stock.Symbol);
                int count = 0;
                int minId = 0;
                if (reader.Read())
                {
                    count = reader.GetInt32(0);
                    if (count > 0)
                    {
                        minId = reader.GetInt32(1);
                    }
                }
                reader.Close();
                if (count > MAX_OLD_ENTRIES_POSIBBLE)
                {
                    RemoveOldEntryFromDatabase(minId);

                }
                AddToDatabase(stock);
                if (_sqlConnection.State != ConnectionState.Closed) { _sqlConnection.Close(); }
            }
            catch (Exception ex) { Debug.WriteLine(ex.ToString()); }

        }

        private SqlDataReader GetOldEntries(string symbol)
        {
            string selectQuery = "SELECT COUNT(*), MIN(id) FROM StockDetails WHERE symbol = @symbol";

            SqlCommand cmd = new SqlCommand(selectQuery, _sqlConnection);
            cmd.Parameters.AddWithValue("@symbol", symbol);

            return cmd.ExecuteReader();
        }

        private void RemoveOldEntryFromDatabase(int idToRemove)
        {
            string deleteQuery = "DELETE FROM StockDetails WHERE id = @idToRemove";

            SqlCommand deleteCmd = new SqlCommand(deleteQuery, _sqlConnection);

            deleteCmd.Parameters.AddWithValue("@idToRemove", idToRemove);
        }

        private void AddToDatabase(Stock stock)
        {
            string addQuery = "INSERT INTO StockDetails ([SYMBOL], [PRICE], [CHANGE]) VALUES (@symbol, @price, @change)";

            SqlCommand cmd = new SqlCommand(addQuery, _sqlConnection);
            cmd.Parameters.AddWithValue("@symbol", stock.Symbol);
            cmd.Parameters.AddWithValue("@price", stock.Price);
            cmd.Parameters.AddWithValue("@change", stock.Change);

            cmd.ExecuteNonQuery();
        }
        public void Dispose()
        {
            _stockLiveUpdateService.StockUpdated -= UpdateDatabase;
        }
    }
}