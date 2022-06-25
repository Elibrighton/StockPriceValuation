using StockPriceValuation.ViewModels;
using System.Windows;

namespace StockPriceValuation.Views
{
    /// <summary>
    /// Interaction logic for StockPriceValuationView.xaml
    /// </summary>
    public partial class StockPriceValuationView : Window
    {
        public StockPriceValuationView(IStockPriceValuationViewModel stockPriceValuationViewModel)
        {
            InitializeComponent();
            DataContext = stockPriceValuationViewModel;
        }
    }
}
