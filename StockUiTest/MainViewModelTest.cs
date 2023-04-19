using NSubstitute;
using StockUi.Model;
using StockUi.Services;
using StockUi.ViewModel;
using System.Windows.Media;


namespace StockUiTest
{
    [TestFixture]
    public class MainViewModelTest
    {
        [SetUp]
        public void Setup()
        {
            _stockUpdateService = Substitute.For<IStockLiveUpdateService>();
            _viewModelUnderTest = new MainViewModel(_stockUpdateService);
        }

        [Test]
        public void PropertiesInitialValue()
        {
            Assert.That(_viewModelUnderTest.Stocks.Count, Is.EqualTo(0));
            Assert.That(_viewModelUnderTest.RaisingStockBrush.ToString(), Is.EqualTo(new SolidColorBrush(Colors.Black).ToString()));
            Assert.That(_viewModelUnderTest.FallingStockBrush.ToString(), Is.EqualTo(new SolidColorBrush(Colors.Black).ToString()));
        }

        [Test]
        public void AddStockWhenNoStocksExists()
        {
            //Arrange
            var newStock = new Stock("stock1", 1, 0);

            //Act
            _stockUpdateService.StockUpdated += Raise.Event<StockUpdatedDelegate>(newStock);

            //Assert
            AssertStockEntry(1, 0, newStock);
        }

        [Test]
        public void UpdateExistingStock()
        {
            //Arrange
            var stock = new Stock("stock1", 1, 0);
            _viewModelUnderTest.Stocks.Add(stock);

            //Act
            var updatedStock = new Stock("stock1", 2, 1);
            _stockUpdateService.StockUpdated += Raise.Event<StockUpdatedDelegate>(updatedStock);

            //Assert
            AssertStockEntry(1, 0, updatedStock);
        }

        [Test]
        public void AddNewStockWhenStocksExist()
        {
            //Arrange
            _viewModelUnderTest.Stocks.Add(new Stock("stock1", 100, 5));
            _viewModelUnderTest.Stocks.Add(new Stock("stock2", 200, -5));

            //Act
            var stock = new Stock("stock3", 300, -10);
            _stockUpdateService.StockUpdated += Raise.Event<StockUpdatedDelegate>(stock);

            //Assert
            AssertStockEntry(3, 2, stock);
        }

        private void AssertStockEntry(int expectedStocksCount, int index, Stock expectedStock)
        {
            Assert.That(_viewModelUnderTest.Stocks.Count(), Is.EqualTo(expectedStocksCount));
            Assert.That(_viewModelUnderTest.Stocks[index].Symbol, Is.EqualTo(expectedStock.Symbol));
            Assert.That(_viewModelUnderTest.Stocks[index].Price, Is.EqualTo(expectedStock.Price));
            Assert.That(_viewModelUnderTest.Stocks[index].Change, Is.EqualTo(expectedStock.Change));
        }

        private MainViewModel _viewModelUnderTest;
        private IStockLiveUpdateService _stockUpdateService;

    }
}