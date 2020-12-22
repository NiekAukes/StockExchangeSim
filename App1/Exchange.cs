﻿using System;
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
        public float Value, stockprice;
        public SynchronizedCollection<StockPriceGraph> stockPrices = new SynchronizedCollection<StockPriceGraph>();
    }

    public class Stock
    {
        public IStockOwner Owner;
        public Company company = null;
        public float value { get { return company.Value * Percentage / 100; } }
        public float tradevalue { get { return company.stockprice * Percentage; } }
        public float SellPrice = 0;
        public float Percentage = 0;
        public float Collected = 0;
        public Stock(Company cp, float percentage)
        {
            //create new stock from company
            company = cp;
            Percentage = percentage;
        }

        public Stock(Stock s, float percentage)
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
        public Stock SplitStock(float percentage)
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
        public void Update(float totalProfit)
        {
            Collected += totalProfit * Percentage * 0.01f;
        }
        public float Collect()
        {
            float ret = Collected;
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

    public interface IExchange
    {
        float money { get; set; }
        List<BidAsk> BidAskSpreads { get; set; }

        bool BuyStock(Company cp, Trader buyer);
        void RegisterCompany(Company cp);
        void RegisterTrader(Trader t);
        void SellStock(Stock stock);
    }
    public class ExchangeBroker : IExchange
    {
        public float money { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<BidAsk> BidAskSpreads { get; set; }
        public List<Company> Companies = new List<Company>(); //lijst van alle geregistreerde bedrijven
        public List<Trader> Traders = new List<Trader>(); //lijst van alle geregistreerde traders
        

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
        public bool BuyStock(Company cp, Trader buyer)
        {
            if (cp.BidAsk == null)
                return false;
            if (cp.BidAsk.Stocks.Count < 1)
                return false;

            money += cp.BidAsk.Bid;
            buyer.money -= cp.BidAsk.Bid;

            cp.BidAsk.Ask *= (float)(cp.BidAsk.Stocks[0].Percentage * 100.0f / (cp.BidAsk.Stocks.Count + 1));
            cp.BidAsk.Bid = cp.BidAsk.Ask * 1.001f;

            cp.BidAsk.Stocks[0].Owner = buyer;
            cp.BidAsk.Stocks.RemoveAt(0);
            return true;
        }

        public void SellStock(Stock stock)
        {
            if (stock.company.BidAsk == null)
                return;

            money += stock.company.BidAsk.Ask;
            stock.Owner.money += stock.company.BidAsk.Ask;

            stock.company.BidAsk.Ask /= (float)(stock.company.BidAsk.Stocks[0].Percentage * 100.0f / (stock.company.BidAsk.Stocks.Count + 1));
            stock.company.BidAsk.Bid = stock.company.BidAsk.Ask * 1.001f;

            stock.Owner = null;
            stock.company.BidAsk.Stocks.Add(stock);
        }
    }
    //doel van exchange is om informatie te verkrijgen en bij te houden. Dit is de class waar andere classes "handelen", net als in een echte exchange
    [Obsolete]
    public class Exchange : IStockOwner, IExchange
    {
        public List<Company> Companies = new List<Company>(); //lijst van alle geregistreerde bedrijven
        List<int> compconverter = new List<int>();
        public List<Trader> Traders = new List<Trader>(); //lijst van alle geregistreerde traders
        public List<BidAsk> BidAskSpreads { get; set; }//Lijst van alle stocks die te koop staan ingedeeld per bedrijf

        public List<Stock> Stocks
        {
            get
            {
                List<Stock> ret = new List<Stock>();
                foreach (var ba in BidAskSpreads)
                {
                    ret.AddRange(ba.Stocks);
                }
                return ret;
            }
            set { }
        }
        public float money { get; set; }

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

        public Exchange() { BidAskSpreads = new List<BidAsk>(); }


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
