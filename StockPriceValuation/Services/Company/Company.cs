namespace StockPriceValuation.Services
{
    public abstract class Company
    {
        public Stock Stock { get; set; }
        public string Name { get; set; }
        public string Industry { get; set; }

        public Company()
        {
            Stock = new Stock();
        }
    }
}
