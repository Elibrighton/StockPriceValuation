using Microsoft.Office.Interop.Excel;
using StockPriceValuation.Base;
using StockPriceValuation.Models;
using StockPriceValuation.Services;
using StockPriceValuation.ViewModels;
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
    public class StockPriceValuationViewModel : ObservableObject, IStockPriceValuationViewModel
    {
        private const int InitialProgressBarMax = 1;
        private const int InitialProgressBarValue = 0;

        private readonly IStockPriceValuationModel _stockPriceValuationModel;

        private ObservableCollection<Company> _companies;
        private bool isCancelled;
        private bool isPaused;

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

        private string _stockCodeTextBox;

        public string StockCodeTextBox
        {
            get { return _stockCodeTextBox; }
            set
            {
                _stockCodeTextBox = value;
                NotifyPropertyChanged("StockCodeTextBox");
            }
        }

        private bool _asxRadioButtonChecked;

        public bool AsxRadioButtonChecked
        {
            get { return _asxRadioButtonChecked; }
            set
            {
                _asxRadioButtonChecked = value;
                NotifyPropertyChanged("AsxRadioButtonChecked");
            }
        }

        private bool _nyseRadioButtonChecked;

        public bool NyseRadioButtonChecked
        {
            get { return _nyseRadioButtonChecked; }
            set
            {
                _nyseRadioButtonChecked = value;
                NotifyPropertyChanged("NyseRadioButtonChecked");
            }
        }

        private bool _nasdaqRadioButtonChecked;

        public bool NasdaqRadioButtonChecked
        {
            get { return _nasdaqRadioButtonChecked; }
            set
            {
                _nasdaqRadioButtonChecked = value;
                NotifyPropertyChanged("NasdaqRadioButtonChecked");
            }
        }

        private bool _asxRadioButtonEnabled;

        public bool AsxRadioButtonEnabled
        {
            get { return _asxRadioButtonEnabled; }
            set
            {
                _asxRadioButtonEnabled = value;
                NotifyPropertyChanged("AsxRadioButtonEnabled");
            }
        }

        private bool _nyseRadioButtonEnabled;

        public bool NyseRadioButtonEnabled
        {
            get { return _nyseRadioButtonEnabled; }
            set
            {
                _nyseRadioButtonEnabled = value;
                NotifyPropertyChanged("NyseRadioButtonEnabled");
            }
        }

        private bool _nasdaqRadioButtonEnabled;

        public bool NasdaqRadioButtonEnabled
        {
            get { return _nasdaqRadioButtonEnabled; }
            set
            {
                _nasdaqRadioButtonEnabled = value;
                NotifyPropertyChanged("NasdaqRadioButtonEnabled");
            }
        }

        private bool _allDecisionComboBox;

        public bool AllDecisionComboBox
        {
            get { return _allDecisionComboBox; }
            set
            {
                _allDecisionComboBox = value;
                NotifyPropertyChanged("AllDecisionComboBox");
            }
        }

        private bool _buyDecisionComboBox;

        public bool BuyDecisionComboBox
        {
            get { return _buyDecisionComboBox; }
            set
            {
                _buyDecisionComboBox = value;
                NotifyPropertyChanged("BuyDecisionComboBox");
            }
        }

        private bool _holdDecisionComboBox;

        public bool HoldDecisionComboBox
        {
            get { return _holdDecisionComboBox; }
            set
            {
                _holdDecisionComboBox = value;
                NotifyPropertyChanged("HoldDecisionComboBox");
            }
        }

        private bool _sellDecisionComboBox;

        public bool SellDecisionComboBox
        {
            get { return _sellDecisionComboBox; }
            set
            {
                _sellDecisionComboBox = value;
                NotifyPropertyChanged("SellDecisionComboBox");
            }
        }

        private bool _unknownDecisionComboBox;

        public bool UnknownDecisionComboBox
        {
            get { return _unknownDecisionComboBox; }
            set
            {
                _unknownDecisionComboBox = value;
                NotifyPropertyChanged("UnknownDecisionComboBox");
            }
        }

        private bool _checkButtonEnabled;

        public bool CheckButtonEnabled
        {
            get { return _checkButtonEnabled; }
            set
            {
                _checkButtonEnabled = value;
                NotifyPropertyChanged("CheckButtonEnabled");
            }
        }

        private bool _pauseButtonEnabled;

        public bool PauseButtonEnabled
        {
            get { return _pauseButtonEnabled; }
            set
            {
                _pauseButtonEnabled = value;
                NotifyPropertyChanged("PauseButtonEnabled");
            }
        }

        private bool _cancelButtonEnabled;

        public bool CancelButtonEnabled
        {
            get { return _cancelButtonEnabled; }
            set
            {
                _cancelButtonEnabled = value;
                NotifyPropertyChanged("CancelButtonEnabled");
            }
        }

        private bool _excludeUnknownCheckbox;

        public bool ExcludeUnknownCheckbox
        {
            get { return _excludeUnknownCheckbox; }
            set
            {
                _excludeUnknownCheckbox = value;
                NotifyPropertyChanged("ExcludeUnknownCheckbox");
            }
        }

        private bool _excludeUnknownCheckboxEnabled;

        public bool ExcludeUnknownCheckboxEnabled
        {
            get { return _excludeUnknownCheckboxEnabled; }
            set
            {
                _excludeUnknownCheckboxEnabled = value;
                NotifyPropertyChanged("ExcludeUnknownCheckboxEnabled");
            }
        }

        public ICommand CheckButtonCommand { get; set; }
        public ICommand PauseButtonCommand { get; set; }
        public ICommand CancelButtonCommand { get; set; }
        public ICommand DecisionChangedCommand { get; set; }
        public ICommand ExcludeUnknownCheckboxCommand { get; set; }

        public StockPriceValuationViewModel(IStockPriceValuationModel stockPriceValuationModel)
        {
            _stockPriceValuationModel = stockPriceValuationModel;
            ResetProgressBar();

            CheckButtonCommand = new RelayCommand(OnCheckButtonCommand);
            PauseButtonCommand = new RelayCommand(OnPauseButtonCommand);
            CancelButtonCommand = new RelayCommand(OnCancelButtonCommand);
            DecisionChangedCommand = new RelayCommand(OnDecisionChangedCommand);
            ExcludeUnknownCheckboxCommand = new RelayCommand(OnExcludeUnknownCheckboxCommand);

            _asxRadioButtonChecked = true;
            _allDecisionComboBox = true;
            _checkButtonEnabled = true;
            _excludeUnknownCheckbox = true;
            _excludeUnknownCheckboxEnabled = true;
            EnableControls();
            ListOfCompanies = new ObservableCollection<Company>();
        }

        private async void OnCheckButtonCommand(object param)
        {
            DisableControls();

            if (isCancelled)
            {
                ListOfCompanies.Clear();
                isCancelled = false;
            }

            if (!isPaused)
            {
                if (_asxRadioButtonChecked)
                {
                    StatusMessageTextBlock = "Downloading spreadsheet";
                    ProgressBarIsIndeterminate = true;

                    var filename = "ASXListedCompanies.csv";
                    var url = string.Concat("https://www.asx.com.au/asx/research/", filename);

                    var excel = await Task.Run(() => DownloadExcel(url, filename));

                    var firstUsedRow = 4;
                    var firstUsedColumn = 1;

                    var range = await Task.Run(() => GetRange(excel, firstUsedRow, firstUsedColumn));

                    ProgressBarIsIndeterminate = false;
                    ProgressBarMax = range.Rows.Count;

                    StatusMessageTextBlock = "Getting ASX companies";
                    _companies = new ObservableCollection<Company>(await Task.Run(() => GetAsxCompanies(excel, range, firstUsedRow, StockCodeTextBox)));
                }
                else if (_nyseRadioButtonChecked || _nasdaqRadioButtonChecked)
                {
                    var path = @"C:\Users\st3gs\Downloads\companylist.csv";

                    if (File.Exists(path))
                    {
                        StatusMessageTextBlock = "Importing spreadsheet";
                        ProgressBarIsIndeterminate = true;

                        var excel = await Task.Run(() => OpenExcel(path));

                        var firstUsedRow = 2;
                        var firstUsedColumn = 1;

                        var range = await Task.Run(() => GetRange(excel, firstUsedRow, firstUsedColumn));

                        ProgressBarIsIndeterminate = false;
                        ProgressBarMax = range.Rows.Count;

                        StatusMessageTextBlock = string.Concat("Getting ", GetUsStockExchangeText(), " companies");
                        _companies = new ObservableCollection<Company>(await Task.Run(() => GetUsCompanies(excel, range, firstUsedRow, StockCodeTextBox)));
                    }
                    else
                    {
                        StatusMessageTextBlock = string.Concat("Spreadsheet does not exist. You can download it from 'https://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=", GetUsStockExchangeText(), "'");
                    }
                }
            }
            else
            {
                isPaused = false;
            }

            if (_companies != null && _companies.Any())
            {
                PauseButtonEnabled = true;
                CancelButtonEnabled = true;
                ResetProgressBar();
                ProgressBarMax = _companies.Count();
                StatusMessageTextBlock = "Valuating stock prices";

                foreach (var company in _companies)
                {
                    if (!isCancelled && !isPaused)
                    {
                        var stock = company.Stock;

                        StatusMessageTextBlock = string.Concat("Valuating ", company.Name);

                        if (string.IsNullOrEmpty(stock.Decision))
                        {
                            await Task.Run(() => GetYahooFinanceResponse(stock));

                            if (stock.HasPrice && stock.HasTtmEps && stock.SecondEps.HasValue)
                            {
                                await Task.Run(() => GetWallStreetJournalResponse(stock));

                                if (stock.FirstEps.HasValue)
                                {
                                    stock.GetEps();

                                    await Task.Run(() => GetMsnMoneyResponse(stock));

                                    if (stock.SecondPeRatio.HasValue)
                                    {
                                        stock.GetPeRatio();
                                    }
                                }
                            }

                            await Task.Run(() => GetStockValuation(stock));

                            if (IsDisplayingCompany(stock))
                            {
                                ListOfCompanies.Add(company);
                            }
                        }

                        ProgressBarValue++;
                    }
                    else
                    {
                        EnableControls();
                    }
                }

                var actionText = GetActionText(isCancelled, isPaused);
                StatusMessageTextBlock = string.Concat(actionText, " valuations");
                ResetProgressBar();
                EnableControls();
                PauseButtonEnabled = false;
                CancelButtonEnabled = false;
            }
        }

        private string GetActionText(bool isCancelled, bool isPaused)
        {
            var actionText = string.Empty;

            if (isCancelled)
            {
                actionText = "Cancelled";
            }
            else if (isPaused)
            {
                actionText = "Paused";
            }
            else
            {
                actionText = "Finished";
            }

            return actionText;
        }

        private bool IsDisplayingCompany(Stock stock)
        {
            return (_allDecisionComboBox && !string.IsNullOrEmpty(stock.Decision) && (stock.Decision != "Unknown" || !_excludeUnknownCheckbox && stock.Decision == "Unknown"))
                        || (_buyDecisionComboBox && stock.Decision == "Buy")
                        || (_holdDecisionComboBox && stock.Decision == "Hold")
                        || (_sellDecisionComboBox && stock.Decision == "Sell")
                        || (_unknownDecisionComboBox && !_excludeUnknownCheckbox && stock.Decision == "Unknown");
        }

        private void EnableControls()
        {
            if (!isPaused)
            {
                AsxRadioButtonEnabled = true;
                NyseRadioButtonEnabled = true;
                NasdaqRadioButtonEnabled = true;
            }

            CheckButtonEnabled = true;
        }

        private void DisableControls()
        {
            AsxRadioButtonEnabled = false;
            NyseRadioButtonEnabled = false;
            NasdaqRadioButtonEnabled = false;
            CheckButtonEnabled = false;
        }

        private void OnPauseButtonCommand(object param)
        {
            HaltValuation("Pausing");
        }

        private void OnCancelButtonCommand(object param)
        {
            HaltValuation("Cancelling");
        }

        private void HaltValuation(string actionText)
        {
            StatusMessageTextBlock = string.Concat(actionText, " valuation");

            if (actionText == "Cancelling")
            {
                isCancelled = true;
            }
            else
            {
                isPaused = true;
            }

            PauseButtonEnabled = false;
            CancelButtonEnabled = false;
        }

        private void OnDecisionChangedCommand(object param)
        {
            if (_allDecisionComboBox)
            {
                ExcludeUnknownCheckboxEnabled = true;
            }
            else
            {
                ExcludeUnknownCheckboxEnabled = false;
                ExcludeUnknownCheckbox = false;
            }

            DisplayCompanies();
        }

        private void OnExcludeUnknownCheckboxCommand(object param)
        {
            if (_allDecisionComboBox)
            {
                DisplayCompanies();
            }
        }

        private void DisplayCompanies()
        {
            if (_companies != null && _companies.Any())
            {
                ListOfCompanies.Clear();

                foreach (var company in _companies)
                {
                    if (IsDisplayingCompany(company.Stock))
                    {
                        ListOfCompanies.Add(company);
                    }
                }
            }
        }

        private string GetUsStockExchangeText()
        {
            return _nyseRadioButtonChecked ? "NYSE" : "NASDAQ";
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

        public ObservableCollection<AusCompany> GetAsxCompanies(Excel excel, Range range, int firstUsedRow, string stockCode)
        {
            var companies = new ObservableCollection<AusCompany>();

            for (var i = firstUsedRow - 1; i < range.Rows.Count; i++)
            {
                var company = new AusCompany();
                company.Name = (string)(excel.Worksheet.Cells[i + 1, 1] as Range).Value;
                company.Stock.Code = (string)(excel.Worksheet.Cells[i + 1, 2] as Range).Value;
                company.Stock.StockExchange = Stock.Exchange.ASX;
                var industry = (string)(excel.Worksheet.Cells[i + 1, 3] as Range).Value;

                if (!_stockPriceValuationModel.Industries.Contains(industry))
                {
                    _stockPriceValuationModel.Industries.Add(industry);
                }

                company.Industry = industry;

                if (string.IsNullOrEmpty(stockCode) || string.Equals(company.Stock.Code, stockCode, StringComparison.OrdinalIgnoreCase))
                {
                    companies.Add(company);
                }

                ProgressBarValue++;

                if (!string.IsNullOrEmpty(stockCode) && string.Equals(company.Stock.Code, stockCode, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            excel.Close();

            return companies;
        }

        public ObservableCollection<UsaCompany> GetUsCompanies(Excel excel, Range range, int firstUsedRow, string stockCode)
        {
            var companies = new ObservableCollection<UsaCompany>();

            for (var i = firstUsedRow - 1; i < range.Rows.Count; i++)
            {
                var company = new UsaCompany();
                company.Name = (string)(excel.Worksheet.Cells[i + 1, 2] as Range).Value;
                company.Stock.Code = (string)(excel.Worksheet.Cells[i + 1, 1] as Range).Value.ToString();
                company.Stock.StockExchange = Stock.Exchange.NYSE;
                var industry = (string)(excel.Worksheet.Cells[i + 1, 8] as Range).Value;

                if (!_stockPriceValuationModel.Industries.Contains(industry))
                {
                    _stockPriceValuationModel.Industries.Add(industry);
                }

                company.Industry = industry;
                var sector = (string)(excel.Worksheet.Cells[i + 1, 7] as Range).Value;

                if (!_stockPriceValuationModel.Sectors.Contains(sector))
                {
                    _stockPriceValuationModel.Sectors.Add(sector);
                }

                company.Sector = sector;

                if (string.IsNullOrEmpty(stockCode) || string.Equals(company.Stock.Code, stockCode, StringComparison.OrdinalIgnoreCase))
                {
                    companies.Add(company);
                }

                ProgressBarValue++;
                
                if (!string.IsNullOrEmpty(stockCode) && string.Equals(company.Stock.Code, stockCode, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            excel.Close();

            return companies;
        }

        public void GetYahooFinanceResponse(Stock stock)
        {
            stock.GetYahooFinanceResponse();
        }

        public void GetWallStreetJournalResponse(Stock stock)
        {
            stock.GetWallStreetJournalResponse();
        }

        public void GetMsnMoneyResponse(Stock stock)
        {
            stock.GetMsnMoneyResponse();
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
            stock.Valuation = new Valuation(_stockPriceValuationModel.Years, _stockPriceValuationModel.RateOfReturn, _stockPriceValuationModel.MarginOfSafety);
            stock.Valuation.TtmEps = stock.TtmEps;
            stock.Valuation.Eps = stock.Eps;
            stock.Valuation.PeRatio = stock.PeRatio;
            stock.Valuation.GetValuation();
            stock.GetDecision();
            stock.GetPercentageDiff();
        }

        private void ResetProgressBar()
        {
            ProgressBarValue = InitialProgressBarValue;
            ProgressBarMax = InitialProgressBarMax;
        }

        #region Class Memebers

        public int ProgressBarMax
        {
            get { return _stockPriceValuationModel.ProgressBarMax; }
            set
            {
                if (_stockPriceValuationModel.ProgressBarMax != value)
                {
                    _stockPriceValuationModel.ProgressBarMax = value;
                    NotifyPropertyChanged("ProgressBarMax");
                }
            }
        }

        public int ProgressBarValue
        {
            get { return _stockPriceValuationModel.ProgressBarValue; }
            set
            {
                if (_stockPriceValuationModel.ProgressBarValue != value)
                {
                    _stockPriceValuationModel.ProgressBarValue = value;
                    NotifyPropertyChanged("ProgressBarValue");
                }
            }
        }

        public bool ProgressBarIsIndeterminate
        {
            get { return _stockPriceValuationModel.ProgressBarIsIndeterminate; }
            set
            {
                if (_stockPriceValuationModel.ProgressBarIsIndeterminate != value)
                {
                    _stockPriceValuationModel.ProgressBarIsIndeterminate = value;
                    NotifyPropertyChanged("ProgressBarIsIndeterminate");
                }
            }
        }

        #endregion
    }
}
