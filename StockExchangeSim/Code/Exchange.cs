﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using StockExchangeSim.Views;

namespace Eco
{
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
        public void Update()
        {
            double modifier = 0.0000001;
            bool loan = false;
            if (company.Value < 100)
            {
                company.Value += 500;
                loan = true;
            }

            Collected +=
                ((company.Competitiveness / company.field.TotalCompetitiveness) * //calculate Competitivenessquote
                //(company.Value * company.Value / company.field.TotalValue) * //calculate Valuequote
                (company.field.MarketShare / Master.TotalShare) * Master.Conjucture //multiply by conjucture
                -
                company.Value * 0.5) * modifier * MainPage.master.SecondsPerTick; //times modifiers
            if (loan)
                company.Value -= 500;
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

    //doel van exchange is om informatie te verkrijgen en bij te houden. Dit is de class waar andere classes "handelen", net als in een echte exchange
    public class Exchange
    {
        private List<Company> Companies = new List<Company>(); //lijst van alle geregistreerde bedrijven
        private List<Trader> Traders = new List<Trader>(); //lijst van alle geregistreerde traders
        public Dictionary<Company, List<Stock>> StocksForSale = new Dictionary<Company, List<Stock>>(); //Lijst van alle stocks die te koop staan ingedeeld per bedrijf

        public List<Company> GetCompanies() { return Companies; }
        public List<Trader> GetTraders() { return Traders; }
        public void RegisterCompany(Company cp)
        {
            Companies.Add(cp);
            StocksForSale.Add(cp, new List<Stock>());
        }
        public void RegisterTrader(Trader t)
        {
            Traders.Add(t);
        }

        public Exchange() { }
        public Exchange(IEnumerable<Company> companies, IEnumerable<Trader> traders)
        {
            Companies = new List<Company>(companies);
            Traders = new List<Trader>(traders);

            for (int i = 0; i < Companies.Count; i++)
            {
                StocksForSale.Add(Companies[i], new List<Stock>());
            }
        }

        public Stock GetCheapestStock(Company cp)
        {
            List<Stock> ls = StocksForSale[cp];
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
            if (!StocksForSale[stock.company].Contains(stock))
            {
                StocksForSale[stock.company].Add(stock);
            }
        }
        public void RevertSellStock(Stock stock)
        {
            if (StocksForSale[stock.company].Contains(stock))
            {
                StocksForSale[stock.company].Remove(stock);
            }
        }
        public void BuyStock(Stock stock, IStockOwner buyer)
        {
            if (StocksForSale[stock.company].Contains(stock))
            {
                buyer.money -= stock.SellPrice;
                stock.Owner.money += stock.SellPrice;

                /*ExchangeDetails? ed = */ExchangeStock(stock, buyer);
            }
        }
        private ExchangeDetails? ExchangeStock(Stock stock, IStockOwner newOwner)
        {

            newOwner.AddStock(stock);

            stock.Owner.UpdateHoldings();
            return null;
        }
    }
}
