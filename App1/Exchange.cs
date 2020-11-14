using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    public class StockPriceGraph
    {
        public float Year { get; set; }
        public float Open { get; set; }
        public float Close { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public StockPriceGraph(float year, float open, float close, float high, float low)
        {
            Year = year;
            Open = open;
            Close = close;
            High = high;
            Low = low;
        }
    }

    public class Company { 
        public double Value, stockprice;
        public ObservableCollection<StockPriceGraph> stockPrices = new ObservableCollection<StockPriceGraph>();
    }

    public class Stock
    {
        public IStockOwner Owner;
        public Company company = null;
        public double value { get { return company.Value * Percentage / 100; } }
        public double tradevalue { get { return company.stockprice * Percentage; } }
        public double SellPrice = 0;
        public double Percentage = 0;
        public double Collected = 0;
        public Stock(Company cp, double percentage)
        {
            //create new stock from company
            company = cp;
            Percentage = percentage;
        }

        public Stock(Stock s, double percentage)
        {
            Percentage = percentage;
            s.Percentage -= percentage;
            company = s.company;
            Owner = s.Owner;
            //create new stock from other stock
        }
        public Stock()
        {

        }
        public Stock SplitStock(double percentage)
        {
            Stock ret = new Stock(this, percentage);
            return ret;
        }
        public void CombineStock(Stock stock)
        {
            if (company != stock.company)
            {
#if DEBUG
                throw new Exception("Stock didn't have same company");
#endif
                return;
            }
            Percentage += stock.Percentage;
        }
        public void Update(double totalProfit)
        {
            Collected += totalProfit * Percentage * 0.01;
        }
        public double Collect()
        {
            double ret = Collected;
            Collected = 0;
            return ret;
        }
    }
    public struct ExchangeDetails
    {

    }
    //bid ask implementatie
    public class BidAsk
    {
        public class AskData
        {
            Trader Asker;
            int amount;
        }
        public Company cp;
        public List<Stock> Bids;
        public List<AskData> Ask;
        public BidAsk(Company company)
        {
            Bids = new List<Stock>();
            Ask = new List<AskData>();
            cp = company;
        }
    }

    //doel van exchange is om informatie te verkrijgen en bij te houden. Dit is de class waar andere classes "handelen", net als in een echte exchange
    public class Exchange
    {
        public List<Company> Companies = new List<Company>(); //lijst van alle geregistreerde bedrijven
        List<int> compconverter = new List<int>();
        public List<Trader> Traders = new List<Trader>(); //lijst van alle geregistreerde traders
        //public List<List<Stock>> StocksForSale = new List<List<Stock>>(); //Lijst van alle stocks die te koop staan ingedeeld per bedrijf
        public List<BidAsk> BidAskSpreads = new List<BidAsk>();

        public void RegisterCompany(Company cp)
        {
            Companies.Add(cp);
            BidAskSpreads.Add(new BidAsk(cp));
        }
        public void RegisterTrader(Trader t)
        {
            Traders.Add(t);
        }

        public Exchange() { }
        //public Exchange(IEnumerable<Company> companies, IEnumerable<Trader> traders)
        //{
        //    Companies = new List<Company>(companies);
        //    Traders = new List<Trader>(traders);

        //    for (int i = 0; i < Companies.Count; i++)
        //    {
        //        StocksForSale.Add(Companies[i], new List<Stock>());
        //    }
        //}

        public Stock GetCheapestStock(Company cp)
        {
            if (!Companies.Contains(cp))
                return null;
            List<Stock> ls = BidAskSpreads[Companies.IndexOf(cp)].Bids;
            Stock Cheapest = null;
            foreach (Stock stock in ls)
            {
                if (Cheapest == null || Cheapest.SellPrice > stock.SellPrice)
                {
                    Cheapest = stock;
                }
            }
            return Cheapest;
        }

        //exchanging stocks
        public void SellStock(Stock stock, double price)
        {
            stock.SellPrice = price;
            List<Stock> ls = BidAskSpreads[Companies.IndexOf(stock.company)].Bids;
            if (!ls.Contains(stock))
            {
                ls.Add(stock);
            }
        }
        public void RevertSellStock(Stock stock)
        {
            List<Stock> ls = BidAskSpreads[Companies.IndexOf(stock.company)].Bids;
            if (!ls.Contains(stock))
            {
                ls.Remove(stock);
            }
        }
        public bool BuyStock(Company cp, IStockOwner buyer)
        {
            if (cp == null)
                return false;
            if (BidAskSpreads[Companies.IndexOf(cp)].Bids.Count > 0)
            {
                //find cheapest stock
                Stock stock = GetCheapestStock(cp);
                buyer.money -= stock.SellPrice;
                stock.Owner.money += stock.SellPrice;

                /*ExchangeDetails? ed = */
                ExchangeStock(stock, buyer);
                return true;
            }
            return false;
        }
        private ExchangeDetails? ExchangeStock(Stock stock, IStockOwner newOwner)
        {

            newOwner.AddStock(stock);

            stock.Owner.UpdateHoldings();
            return null;
        }
    }
}
