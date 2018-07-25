using Microsoft.Office.Interop.Excel;
using StockPriceValuation.Base;
using StockPriceValuation.Models;
using StockPriceValuation.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ObservableCollection<Company> _listOfCompanies;

        public ObservableCollection<Company> ListOfCompanies
        {
            get { return _listOfCompanies; }
            set
            {
                _listOfCompanies = value;
                NotifyPropertyChanged("ListOfCompanies");
            }
        }

        public ICommand CheckAsxButtonCommand { get; set; }
        public ICommand CheckNyseButtonCommand { get; set; }
        public ICommand CheckNasdaqButtonCommand { get; set; }

        public StockPriceValuationViewModel()
        {
            CheckAsxButtonCommand = new RelayCommand(OnCheckAsxButtonCommand);
            CheckNyseButtonCommand = new RelayCommand(OnCheckNyseButtonCommand);
            CheckNasdaqButtonCommand = new RelayCommand(OnCheckNasdaqButtonCommand);
            ResetMainProgress();
            _stockPriceValuation.Years = 10;
            _stockPriceValuation.RateOfReturn = 0.15; // 15%
            _stockPriceValuation.MarginOfSafety = 0.50; // 50%
            ListOfCompanies = new ObservableCollection<Company>();

        }

        private async void OnCheckAsxButtonCommand(object param)
        {
            StatusMessageTextBlock = "Downloading spreadsheet";
            MainProgressIsIndeterminate = true;

            var filename = "ASXListedCompanies.csv";
            var url = string.Concat("https://www.asx.com.au/asx/research/", filename);

            var excel = await Task.Run(() => DownloadExcel(url, filename));

            var firstUsedRow = 4;
            var firstUsedColumn = 1;

            var range = await Task.Run(() => GetRange(excel, firstUsedRow, firstUsedColumn));

            MainProgressIsIndeterminate = false;
            MainProgressMax = range.Rows.Count;

            StatusMessageTextBlock = "Getting ASX companies";
            var asxCompanies = await Task.Run(() => GetAsxCompanies(excel, range, firstUsedRow));

            ResetMainProgress();
            MainProgressMax = asxCompanies.Count();
            StatusMessageTextBlock = "Valuating stock prices";

            foreach (var company in asxCompanies)
            {
                var stock = company.Stock;

                await Task.Run(() => GetStockPrice(stock));

                if (stock.HasPrice)
                {
                    await Task.Run(() => GetStockTtmEps(stock));

                    if (stock.HasTtmEps)
                    {
                        await Task.Run(() => GetStockEps(stock));

                        if (stock.HasEps)
                        {
                            await Task.Run(() => GetStockPeRatio(stock));

                            if (stock.HasPeRatio)
                            {
                                await Task.Run(() => GetStockValuation(stock));
                            }
                        }
                    }
                }

                MainProgressValue++;

                if (stock.Decision == "Buy" && stock.HasPrice && stock.HasTtmEps && stock.HasEps && stock.HasPeRatio)
                {
                    ListOfCompanies.Add(company);
                }
            }

            StatusMessageTextBlock = "Finished update";
            ResetMainProgress();
        }
        private async void OnCheckNyseButtonCommand(object param)
        {
            var path = @"C:\Users\st3gs\Downloads\companylist.csv";

            if (File.Exists(path))
            {
                StatusMessageTextBlock = "Importing spreadsheet";
                MainProgressIsIndeterminate = true;

                var excel = await Task.Run(() => OpenExcel(path));

                var firstUsedRow = 2;
                var firstUsedColumn = 1;

                var range = await Task.Run(() => GetRange(excel, firstUsedRow, firstUsedColumn));

                MainProgressIsIndeterminate = false;
                MainProgressMax = range.Rows.Count;

                StatusMessageTextBlock = "Getting NYSE companies";
                var nyseCompanies = await Task.Run(() => GetNyseCompanies(excel, range, firstUsedRow));

                ResetMainProgress();
                MainProgressMax = nyseCompanies.Count();
                StatusMessageTextBlock = "Valuating stock prices";
                foreach (var company in nyseCompanies)
                {
                    var stock = company.Stock;

                    await Task.Run(() => GetStockSales(stock));

                    MainProgressValue++;
                    ListOfCompanies.Add(company);
                }

                StatusMessageTextBlock = "Finished update";
                ResetMainProgress();
            }
            else
            {
                StatusMessageTextBlock = "Spreadsheet does not exist";
            }
        }

        private async void OnCheckNasdaqButtonCommand(object param)
        {

        }

        public Excel OpenExcel(string path)
        {
            return new Excel(path);
        }

        public Excel DownloadExcel(string url, string filename)
        {
            var path = string.Concat(Path.GetTempPath(), @"\", filename);

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

        public Range GetRange(Excel excel, int firstUsedRow, int firstUsedColumn)
        {
            return excel.GetRange(firstUsedRow, firstUsedColumn);
        }

        public ObservableCollection<AusCompany> GetAsxCompanies(Excel excel, Range range, int firstUsedRow)
        {
            var companies = new ObservableCollection<AusCompany>();

            for (var i = firstUsedRow - 1; i < range.Rows.Count; i++)
            {
                var company = new AusCompany();
                company.Name = (string)(excel.Worksheet.Cells[i + 1, 1] as Range).Value;
                company.Stock.Code = (string)(excel.Worksheet.Cells[i + 1, 2] as Range).Value;
                company.Stock.StockExchange = Stock.Exchange.ASX;
                var industry = (string)(excel.Worksheet.Cells[i + 1, 3] as Range).Value;

                if (!_stockPriceValuation.Industries.Contains(industry))
                {
                    _stockPriceValuation.Industries.Add(industry);
                }

                company.Industry = industry;
                companies.Add(company);
                MainProgressValue++;
            }

            excel.Close();

            return companies;
        }

        public ObservableCollection<UsaCompany> GetNyseCompanies(Excel excel, Range range, int firstUsedRow)
        {
            var companies = new ObservableCollection<UsaCompany>();

            for (var i = firstUsedRow - 1; i < range.Rows.Count; i++)
            {
                var company = new UsaCompany();
                company.Name = (string)(excel.Worksheet.Cells[i + 1, 2] as Range).Value;
                company.Stock = new UsaStock();
                company.Stock.Code = (string)(excel.Worksheet.Cells[i + 1, 1] as Range).Value;
                company.Stock.StockExchange = Stock.Exchange.NYSE;
                var industry = (string)(excel.Worksheet.Cells[i + 1, 8] as Range).Value;

                if (!_stockPriceValuation.Industries.Contains(industry))
                {
                    _stockPriceValuation.Industries.Add(industry);
                }

                company.Industry = industry;
                var sector = (string)(excel.Worksheet.Cells[i + 1, 7] as Range).Value;

                if (!_stockPriceValuation.Sectors.Contains(sector))
                {
                    _stockPriceValuation.Sectors.Add(sector);
                }

                company.Sector = sector;
                companies.Add(company);
                MainProgressValue++;
            }

            excel.Close();

            return companies;
        }

        public void GetStockSales(UsaStock stock)
        {
            stock.GetSales();
        }

        public void GetStockPrice(AusStock stock)
        {
            stock.GetPrice();
        }

        public void GetStockTtmEps(AusStock stock)
        {
            stock.GetTtmEps();
        }

        public void GetStockEps(AusStock stock)
        {
            stock.GetEps();
        }

        public void GetStockPeRatio(AusStock stock)
        {
            stock.GetPeRatio();
        }

        public void GetStockValuation(AusStock stock)
        {
            stock.Valuation = new Valuation(_stockPriceValuation.Years, _stockPriceValuation.RateOfReturn, _stockPriceValuation.MarginOfSafety);
            stock.Valuation.TtmEps = stock.TtmEps;
            stock.Valuation.Eps = stock.Eps;
            stock.Valuation.PeRatio = stock.PeRatio;
            stock.Valuation.GetValuation();
            stock.GetDecision();
        }

        private void ResetMainProgress()
        {
            MainProgressValue = 0;
            MainProgressMax = 1;
        }
    }
}
