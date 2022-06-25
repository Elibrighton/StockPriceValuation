using StockPriceValuation.Models;
using StockPriceValuation.ViewModels;
using StockPriceValuation.Views;
using System.Windows;
using Unity;

namespace StockPriceValuation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IStockPriceValuationViewModel, StockPriceValuationViewModel>();
            container.RegisterType<IStockPriceValuationModel, StockPriceValuationModel>();
            //container.RegisterType<ICompany, Company>();
            //container.RegisterType<IStock, Stock>();

            var window = container.Resolve<StockPriceValuationView>();
            window.Show();
        }
    }
}
