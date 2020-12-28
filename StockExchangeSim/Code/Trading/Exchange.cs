using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Eco
{


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
    public class Liquidity
    {
        public float Year { get; set; }
        public int SellAmount { get; set; }
        public int BuyAmount { get; set; }
        public int Diff { get { return BuyAmount - SellAmount; } }
        public Liquidity(float year)
        {
            Year = year;
        }

        public static Liquidity operator+(Liquidity lq1, Liquidity lq2)
        {
            Liquidity ret = new Liquidity(lq1.Year);
            ret.BuyAmount = lq1.BuyAmount + lq2.BuyAmount;
            ret.SellAmount = lq1.SellAmount + lq2.SellAmount;

            return ret;
        }
    }
    
    public class ECNBroker : IExchange
    {
        public float money { get; set; }

        public List<BidAsk> BidAsks { get; set; }



        public List<Company> Companies = new List<Company>(); //lijst van alle geregistreerde bedrijven
        public List<Trader> Traders = new List<Trader>(); //lijst van alle geregistreerde traders

        

        public bool BuyStock(Company cp, Trader buyer)
        {
            if (cp.BidAsk == null)
                return false;
            if (cp.SellOrders.list.Count < 1)
                return false;

            SellOrder SellStock = cp.SellOrders.list[0];
            cp.SellOrders.list.RemoveAt(0);
            SellStock.Stock.Owner.money += SellStock.LimitPrice;
            buyer.money -= SellStock.LimitPrice;

           

            SellStock.Stock.Owner = buyer;

            //cp.BidAsk.liquidity1m[cp.BidAsk.liquidity1m.Count - 1].BuyAmount++;
            return true;
        }

        public void BuyOrder(Company cp, Trader buyer, float Limit)
        {
            if (cp.SellOrders.list[0].LimitPrice < Limit)
            {
                //directly issue transaction
                BuyStock(cp, buyer);
                return;
            }
            cp.BuyOrders.AddSortedItem(new BuyOrder(cp, buyer, Limit), "LimitPrice");
        }
        public void SellStock(Stock stock)
        {
            throw new NotImplementedException();
        }

        public void SellOrder(Stock stock, float Limit)
        {
            if (stock.company.BuyOrders.list[0].LimitPrice > Limit)
            {
                //directly issue transaction
                SellStock(stock);
                return;
            }
            stock.company.SellOrders.AddSortedItem(new Eco.SellOrder(stock.company, stock, Limit), "LimitPrice");
        }
        public void RegisterCompany(Company cp, int partition)
        {
            Companies.Add(cp);
            BidAsk bidAsk = new BidAsk(cp); //create new bidask
            BidAsks.Add(bidAsk);
            cp.BidAsk = bidAsk;

            //buy stocks from company => to Inverstors
            //Stock FullbuyStock = cp.BecomePublic();
            //for (int i = 0; i < partition - 1; i++)
            //{
            //    bidAsk.Stocks.Add(FullbuyStock.SplitStock(1.0f / partition));
            //}
            //bidAsk.Stocks.Add(FullbuyStock);

            //bidAsk.liquidity1m.Add(new Liquidity(Master.inst.Year) { BuyAmount = partition, SellAmount = 0, Year = Master.inst.Year });

            //bidAsk.Bid = cp.Value * FullbuyStock.Percentage * 0.01f;
            //bidAsk.Ask = bidAsk.Bid * 0.0098f;
            //cp.Value += cp.Value * FullbuyStock.Percentage * partition * 0.01f;
        }

        public void RegisterTrader(Trader t)
        {
            Traders.Add(t);
        }

        
    }
    public class ExchangeBrokerMM : IExchange
    {
        public float money { get; set; }
        public List<OLD_BidAsk> BidAskSpreads { get; set; }
        public List<Company> Companies = new List<Company>(); //lijst van alle geregistreerde bedrijven
        public List<Trader> Traders = new List<Trader>(); //lijst van alle geregistreerde traders

        public ExchangeBrokerMM()
        {
            BidAskSpreads = new List<OLD_BidAsk>();
        }
        public void RegisterCompany(Company cp, int partition)
        {
            Companies.Add(cp);
            OLD_BidAsk bidAsk = new OLD_BidAsk(cp); //create new bidask
            BidAskSpreads.Add(bidAsk);
            cp.BidAsk = bidAsk;

            //buy stocks from company
            Stock FullbuyStock = cp.BecomePublic();
            for(int i = 0; i < partition - 1; i++)
            {
                bidAsk.Stocks.Add(FullbuyStock.SplitStock(1.0f / partition));
            }
            bidAsk.Stocks.Add(FullbuyStock);

            bidAsk.liquidity1m.Add(new Liquidity(Master.inst.Year) { BuyAmount = partition, SellAmount = 0, Year = Master.inst.Year });

            bidAsk.Bid = cp.Value * FullbuyStock.Percentage * 0.01f;
            bidAsk.Ask = bidAsk.Bid * 0.0098f;
            cp.Value += cp.Value * FullbuyStock.Percentage * partition * 0.01f;


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

            cp.BidAsk.liquidity1m[cp.BidAsk.liquidity1m.Count - 1].BuyAmount++;
            return true;
        }

        public void SellStock(Stock stock)
        {
            if (stock.company.BidAsk == null)
                return;

            money += stock.company.BidAsk.Ask;
            stock.Owner.money += stock.company.BidAsk.Ask;

            stock.company.BidAsk.Ask /= (stock.company.BidAsk.Stocks[0].Percentage * 100.0f / (stock.company.BidAsk.Stocks.Count + 1));
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
        public List<OLD_BidAsk> BidAskSpreads { get; set; }//Lijst van alle stocks die te koop staan ingedeeld per bedrijf

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

        public void RegisterCompany(Company cp ,int part)
        {
            Companies.Add(cp);
            OLD_BidAsk bidAsk = new OLD_BidAsk(cp); //create new bidask
            BidAskSpreads.Add(bidAsk);
            cp.BidAsk = bidAsk;
        }
        public void RegisterTrader(Trader t)
        {
            Traders.Add(t);
        }

        public Exchange() { BidAskSpreads = new List<OLD_BidAsk>(); }


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
