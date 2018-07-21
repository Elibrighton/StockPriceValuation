using Microsoft.Office.Interop.Excel;
using StockPriceValuation.Base;
using StockPriceValuation.Models;
using StockPriceValuation.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockPriceValuation
{
    public class StockPriceValuationViewModel : ObservableObject
    {
        private StockPriceValuationModel _stockPriceValuation = new StockPriceValuationModel();
        private int _years;
        private double _rateOfReturn;
        private double _marginOfSafety;

        private string _lblPriceToBuyContent;

        public string LblPriceToBuyContent
        {
            get { return _lblPriceToBuyContent; }
            set
            {
                _lblPriceToBuyContent = value;
                _stockPriceValuation.PriceToBuyContent = _lblPriceToBuyContent;
                NotifyPropertyChanged("LblPriceToBuyContent");
            }
        }

        private int _mainProgressMax;

        public int MainProgressMax
        {
            get { return _mainProgressMax; }
            set
            {
                if (_mainProgressMax != value)
                {
                    _mainProgressMax = value;
                    NotifyPropertyChanged("MainProgressMax");
                }
            }
        }

        private int _mainProgressValue;

        public int MainProgressValue
        {
            get { return _mainProgressValue; }
            set
            {
                if (_mainProgressValue != value)
                {
                    _mainProgressValue = value;
                    NotifyPropertyChanged("MainProgressValue");
                }
            }
        }

        private bool _mainProgressIsIndeterminate;

        public bool MainProgressIsIndeterminate
        {
            get { return _mainProgressIsIndeterminate; }
            set
            {
                if (_mainProgressIsIndeterminate != value)
                {
                    _mainProgressIsIndeterminate = value;
                    NotifyPropertyChanged("MainProgressIsIndeterminate");
                }
            }
        }

        private string _statusMessageTextBlock;

        public string StatusMessageTextBlock
        {
            get { return _statusMessageTextBlock; }
            set
            {
                _statusMessageTextBlock = value;
                NotifyPropertyChanged("StatusMessageTextBlock");
            }
        }

        public ICommand GetPriceToBuyButtonCommand { get; set; }

        public StockPriceValuationViewModel()
        {
            LblPriceToBuyContent = "$0";
            GetPriceToBuyButtonCommand = new RelayCommand(OnGetPriceToBuyButtonCommand);
            ResetMainProgress();
            _years = 10;
            _rateOfReturn = 0.15; // 15%
            _marginOfSafety = 0.50; // 50%
        }

        private async void OnGetPriceToBuyButtonCommand(object param)
        {
            StatusMessageTextBlock = "Downloading spreadsheet";
            MainProgressIsIndeterminate = true;

            var excel = await Task.Run(() => GetExcel());
            var range = await Task.Run(() => GetRange(excel));

            MainProgressIsIndeterminate = false;
            MainProgressMax = range.Rows.Count;

            StatusMessageTextBlock = "Getting ASX companies";
            var asxCompanies = await Task.Run(() => GetAsxCompanies(excel, range));

            ResetMainProgress();
            MainProgressMax = asxCompanies.Count();
            StatusMessageTextBlock = "Getting stock prices";

            foreach (var company in asxCompanies)
            {
                var stock = company.Stock;

                await Task.Run(() => GetStockPrice(stock));
                await Task.Run(() => GetStockTtmEps(stock));
                await Task.Run(() => GetStockEps(stock));
                await Task.Run(() => GetStockPeRatio(stock));

                if (stock.HasTtmEps && stock.HasEps && stock.HasPeRatio)
                {
                    await Task.Run(() => GetStockValuation(stock));
                }

                MainProgressValue++;
            }
        }

        public Excel GetExcel()
        {
            var url = "https://www.asx.com.au/asx/research/ASXListedCompanies.csv";
            var path = string.Concat(Path.GetTempPath(), @"\ASXListedCompanies.csv");

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            // download spreadsheet 
            using (var memoryStream = Web.GetStream(url))
            {
                var excel = new Excel(path);

                // write stream to file
                using (var fileStream = File.Create(path))
                {
                    memoryStream.CopyTo(fileStream);
                }

                return excel;
            }
        }

        public Range GetRange(Excel excel)
        {
            return excel.GetRange();
        }

        public List<Company> GetAsxCompanies(Excel excel, Range range)
        {
            var asxCompanies = new List<Company>();

            for (var i = 3; i < range.Rows.Count; i++)
            {
                var asxCompany = new Company();
                asxCompany.Name = (string)(excel.Worksheet.Cells[i + 1, 1] as Range).Value;
                asxCompany.Stock = new Stock();
                asxCompany.Stock.Code = (string)(excel.Worksheet.Cells[i + 1, 2] as Range).Value;
                asxCompany.Stock.StockExchange = Stock.Exchange.ASX;
                asxCompany.Industry = Company.GetIndustry((string)(excel.Worksheet.Cells[i + 1, 3] as Range).Value);
                asxCompanies.Add(asxCompany);
                MainProgressValue++;
            }

            excel.Close();

            return asxCompanies;
        }

        public void GetStockPrice(Stock stock)
        {
            stock.GetPrice();
        }

        public void GetStockTtmEps(Stock stock)
        {
            stock.GetTtmEps();
        }

        public void GetStockEps(Stock stock)
        {
            stock.GetEps();
        }

        public void GetStockPeRatio(Stock stock)
        {
            stock.GetPeRatio();
        }

        public void GetStockValuation(Stock stock)
        {
            stock.Valuation = new Valuation(_years, _rateOfReturn, _marginOfSafety);
            stock.Valuation.TtmEps = stock.TtmEps;
            stock.Valuation.Eps = stock.Eps;
            stock.Valuation.PeRatio = stock.PeRatio;
            stock.Valuation.GetValuation();
        }

        private void ResetMainProgress()
        {
            MainProgressValue = 0;
            MainProgressMax = 1;
        }
    }
}
