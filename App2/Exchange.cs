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
        public BidAsk BidAsk;
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
        public List<Stock> Stocks = new List<Stock>();
        public class AskData
        {
            Trader Asker;
            int amount;
        }
        public Company cp;
        public float Bid, Ask;
        //public List<Stock> Bids;
        //public List<AskData> Ask;
        public BidAsk(Company company)
        {
            //Bids = new List<Stock>();
            //Ask = new List<AskData>();
            cp = company;
        }
    }

    //doel van exchange is om informatie te verkrijgen en bij te houden. Dit is de class waar andere classes "handelen", net als in een echte exchange
    public class Exchange : IStockOwner
    {
        public List<Company> Companies = new List<Company>(); //lijst van alle geregistreerde bedrijven
        List<int> compconverter = new List<int>();
        public List<Trader> Traders = new List<Trader>(); //lijst van alle geregistreerde traders
        public List<BidAsk> BidAskSpreads = new List<BidAsk>();//Lijst van alle stocks die te koop staan ingedeeld per bedrijf

        public List<Stock> Stocks { get {
                List<Stock> ret = new List<Stock>();
                foreach(var ba in BidAskSpreads)
                {
                    ret.AddRange(ba.Stocks);
                }
                return ret;
            } set { } }
        public double money { get; set; }

        public void RegisterCompany(Company cp)
        {
            Companies.Add(cp);
            BidAsk bidAsk = new BidAsk(cp); //create new bidask
            BidAskSpreads.Add(bidAsk);
            cp.BidAsk = bidAsk;
        }
        public void RegisterTrader(Trader t)
        {
            Traders.Add(t);
        }

        public Exchange() { }

        
        Stock GetCheapestStock(Company cp)
        {
            if (!Companies.Contains(cp))
                return null;

            
            List<Stock> ls = BidAskSpreads[Companies.IndexOf(cp)].Stocks;
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
        public void SellStock(Stock stock)
        {
            List<Stock> ls = stock.company.BidAsk.Stocks;
            if (!ls.Contains(stock))
            {
                ls.Add(stock);
                stock.Owner = null;
            }
        }
        public bool BuyStock(Company cp, Trader buyer)
        {
            if (cp == null)
                return false;
            if (cp.BidAsk.Stocks.Count > 0)
            {
                //find cheapest stock
                Stock stock = cp.BidAsk.Stocks[0];
                cp.BidAsk.Stocks.RemoveAt(0);
                buyer.money -= cp.BidAsk.Bid;
                money += cp.BidAsk.Bid;

                //complete transaction
                buyer.AddStock(stock);

                cp.BidAsk.Bid *= 1.001f;
                
                return true;
            }
            return false;
        }
    }
}
