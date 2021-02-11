using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Eco
{


    public class Stock
    {
        public Trader Owner;
        public Company company = null;
        public float value { get { return company.Value * Percentage / 100; } }
        public float tradevalue { get { return company.stockprice * Percentage; } }
        public float SellPrice = 0;
        public float Percentage = 0;
        public float Collected = 0;
        public bool isCompanystock = false;
        public Stock(Company cp, float percentage)
        {
            //create new stock from company
            company = cp;
            Percentage = percentage;
            isCompanystock = true;
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
        public Stock SplitStock(float percentage, Trader buyer)
        {
            Stock ret = new Stock(this, percentage);
            ret.Owner = buyer;
            return ret;
        }
        public void CombineStock(Stock stock)
        {
            if (company != stock.company)
            {
                throw new Exception("Stock didn't have same company");
            }
            Percentage += stock.Percentage;
        }
        public void Update(double totalProfit)
        {
            if (isCompanystock)
                company.dValue += totalProfit * Percentage * 0.01;
            else
                Owner.money += (float)totalProfit * Percentage * 0.01f;
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
        public List<Holder> holders = new List<Holder>(); //lijst van alle marketmaker holders

        public ECNBroker()
        {
            BidAsks = new List<BidAsk>();
        }

        public float GetCheapestHolderBid(Company cp)
        {
            float cheapest = 10000000.0f;
            for (int i = 0; i < holders.Count; i++)
            {
                if (holders[i].bidask.cp == cp)
                {
                    if (holders[i].bidask.Bid < cheapest && holders[i].Stocks.Count > 1)
                    {
                        cheapest = holders[i].bidask.Bid;
                    }
                }
            }
            return cheapest;
        }
        public float GetCheapestSeller(Company cp)
        {
            float cheapest = float.MaxValue;
            for (int i = 0; i < cp.SellOrders.Count; i++)
            {
                try
                {
                    if (cp.SellOrders[i].LimitPrice < cheapest)
                    {
                        cheapest = cp.SellOrders[i].LimitPrice;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return cheapest;
        }
        public bool BuyStock(Company cp, Trader buyer)
        {
            if (cp.BidAsk == null)
                return false;

            float cheapest = float.MaxValue;
            SellOrder SellStock = null;
            int cheapestholder = -1;
            try
            {

                for (int i = 0; i < cp.SellOrders.Count; i++)
                {
                    if (cp.SellOrders[i].LimitPrice < cheapest)
                    {
                        cheapest = cp.SellOrders[i].LimitPrice;
                        SellStock = cp.SellOrders[i];
                    }
                }

                for (int i = 0; i < holders.Count; i++)
                {
                    if (holders[i].bidask.cp == cp)
                    {
                        if (holders[i].bidask.Bid < cheapest &&
                            holders[i].MaxStockLimit > holders[i].Stocks.Count &&
                            holders[i].Stocks.Count > 1)
                        {
                            cheapest = holders[i].bidask.Bid;
                            cheapestholder = i;
                        }
                    }
                }
            }

            catch (Exception e)
            {
                Debug.WriteLine("something went wrong in checking; ");
                return false;
            }
            if (cheapestholder >= 0)
            {
                //buy from holder
                lock (holders[cheapestholder].Stocks)
                {
                    if (holders[cheapestholder].Stocks.Count > 0)
                    {
                        holders[cheapestholder].Owner.money += cheapest;
                        buyer.money -= cheapest;

                        //cp.stockprice = cheapest;

                        holders[cheapestholder].Stocks[0].Owner = buyer;
                        holders[cheapestholder].Stocks.RemoveAt(0);
                        holders[cheapestholder].OnStockTraded(null);
                    }
                }
                return true;
            }

            try
            {
                if (SellStock == null)
                    return false;

                lock (SellStock.Stock)
                {
                    if (SellStock.Amount < 0 || SellStock.Stock.Count < 1)
                        return false; //transaction failed
                    SellStock.Amount--;
                    if (SellStock.Amount < 1)
                        cp.SellOrders.Remove(SellStock);

                    
                    SellStock.Stock[0].Owner.money += SellStock.LimitPrice;

                    buyer.money -= SellStock.LimitPrice;

                    cp.stockprice = SellStock.LimitPrice;

                    SellStock.Stock[0].Owner = buyer;
                    buyer.AddStock(SellStock.Stock[0]);
                    //remove stock from original owner and move to new owner
                    SellStock.Stock.RemoveAt(0);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("another thing went wrong in transaction");
            }
            return true;
        }

        public BuyOrder buyOrder(Company cp, Trader buyer, float Limit, int amount = 1)
        {
            for (int i = 0; i < holders.Count; i++)
            {
                if (holders[i].bidask.cp == cp && holders[i].MaxStockLimit > holders[i].Stocks.Count)
                {
                    if (holders[i].bidask.Bid < Limit)
                    {
                        BuyStock(cp, buyer);
                        return null;
                    }
                }
            }

            float cheapest = float.MaxValue;
            SellOrder SellStock = null;

            for (int i = 0; i < cp.SellOrders.Count; i++)
            {
                try
                {
                    if (cp.SellOrders[i].LimitPrice < cheapest)
                    {
                        cheapest = cp.SellOrders[i].LimitPrice;
                        SellStock = cp.SellOrders[i];
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            if (cheapest < Limit)
            {
                //directly issue transaction
                BuyStock(cp, buyer);
                return null;
            }
            BuyOrder ret = new BuyOrder(cp, buyer, Limit, amount);
            
            cp.BuyOrders.Add(ret);
            return ret;
        }
        public void SellStock(Stock stock)
        {
            if (stock.company.BidAsk == null)
                return;

            Company cp = stock.company;

            float best = float.MaxValue;
            BuyOrder order = null;
                for (int i = 0; i < cp.BuyOrders.Count; i++)
                {
                    if (cp.BuyOrders[i].LimitPrice < best)
                    {
                        best = cp.BuyOrders[i].LimitPrice;
                        order = cp.BuyOrders[i];
                    }
                }
            int bestholder = -1;
                for (int i = 0; i < holders.Count; i++)
                {
                    if (holders[i].bidask.cp == stock.company)
                    {
                        if (holders[i].bidask.Ask < best)
                        {
                            best = holders[i].bidask.Ask;
                            bestholder = i;
                        }
                    }
                }

            if (bestholder >= 0)
            {
                //sell to holder
                holders[bestholder].Owner.money -= best;
                stock.Owner.money += best;
                stock.Owner.stocks[stock.Owner.InterestedCompanies.IndexOf(cp)].Remove(stock);
                stock.Owner = holders[bestholder].Owner;
                holders[bestholder].Stocks.Add(stock);

                holders[bestholder].OnStockTraded(null);


                return;
            }
            if (stock.company.BuyOrders.Count < 1)
                return;

            order.Buyer.money -= order.LimitPrice;
            stock.Owner.money += order.LimitPrice;

            stock.company.stockprice = order.LimitPrice;

            //transfer ownership
            stock.Owner = order.Buyer;
        }

        public SellOrder sellOrder(List<Stock> stocklist, Company cp, float Limit, int amount = 1)
        {
            for (int i = 0; i < holders.Count; i++)
            {
                while(amount > 0)
                {
                    if (holders[i].bidask.Ask > Limit && holders[i].MaxStockLimit < holders[i].Stocks.Count)
                    {
                        SellStock(stocklist[0]);
                        amount--;
                        return null;
                    }
                    else
                        break;
                }
            }
            float best = float.MaxValue;
            BuyOrder order = null;
                for (int i = 0; i < cp.BuyOrders.Count; i++)
                {
                    if (cp.BuyOrders[i].LimitPrice < best)
                    {
                        best = cp.BuyOrders[i].LimitPrice;
                        order = cp.BuyOrders[i];
                    }
                }
            if (best > Limit && stocklist.Count > 0)
            {
                //directly issue transaction
                SellStock(stocklist[0]);
                return null;
            }
            SellOrder ret = new Eco.SellOrder(cp, stocklist, Limit);
            cp.SellOrders.Add(ret);
            return ret;
        }

        public void CheckBuyOrder(BuyOrder order)
        {
            Company cp = order.cp;
            float Limit = order.LimitPrice;
            for (int i = 0; i < holders.Count; i++)
            {
                if (holders[i].bidask.cp == cp && holders[i].MaxStockLimit > holders[i].Stocks.Count)
                {
                    if (holders[i].bidask.Bid < Limit)
                    {
                        BuyStock(cp, order.Buyer);
                        return;
                    }
                }
            }

            float cheapest = float.MaxValue;
            SellOrder SellStock = null;

            for (int i = 0; i < cp.SellOrders.Count; i++)
            {
                try
                {
                    if (cp.SellOrders[i].LimitPrice < cheapest)
                    {
                        cheapest = cp.SellOrders[i].LimitPrice;
                        SellStock = cp.SellOrders[i];
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            if (cheapest < Limit)
            {
                //directly issue transaction
                BuyStock(cp, order.Buyer);
                return;
            }
        }

        public void RegisterCompany(Company cp, int partition)
        {
            Companies.Add(cp);
            BidAsk bidAsk = new BidAsk(cp); //create new bidask
            BidAsks.Add(bidAsk);
            cp.BidAsk = bidAsk;

        }
        public void RegisterHolder(Holder hd)
        {
            holders.Add(hd);
        }

        public void RegisterTrader(Trader t)
        {
            Traders.Add(t);
        }

        
    }
    //public class ExchangeBrokerMM : IExchange
    //{
    //    public float money { get; set; }
    //    public List<OLD_BidAsk> BidAskSpreads { get; set; }
    //    public List<Company> Companies = new List<Company>(); //lijst van alle geregistreerde bedrijven
    //    public List<Trader> Traders = new List<Trader>(); //lijst van alle geregistreerde traders

    //    public ExchangeBrokerMM()
    //    {
    //        BidAskSpreads = new List<OLD_BidAsk>();
    //    }
    //    public void RegisterCompany(Company cp, int partition)
    //    {
    //        Companies.Add(cp);
    //        OLD_BidAsk bidAsk = new OLD_BidAsk(cp); //create new bidask
    //        BidAskSpreads.Add(bidAsk);
    //        cp.BidAsk = bidAsk;

    //        //buy stocks from company
    //        Stock FullbuyStock = cp.BecomePublic();
    //        for(int i = 0; i < partition - 1; i++)
    //        {
    //            bidAsk.Stocks.Add(FullbuyStock.SplitStock(1.0f / partition));
    //        }
    //        bidAsk.Stocks.Add(FullbuyStock);

    //        bidAsk.liquidity1m.Add(new Liquidity(Master.inst.Year) { BuyAmount = partition, SellAmount = 0, Year = Master.inst.Year });

    //        bidAsk.Bid = cp.Value * FullbuyStock.Percentage * 0.01f;
    //        bidAsk.Ask = bidAsk.Bid * 0.0098f;
    //        cp.Value += cp.Value * FullbuyStock.Percentage * partition * 0.01f;


    //    }

    //    public void RegisterTrader(Trader t)
    //    {
    //        Traders.Add(t);
    //    }
    //    public bool BuyStock(Company cp, Trader buyer)
    //    {
    //        if (cp.BidAsk == null)
    //            return false;
    //        if (cp.BidAsk.Stocks.Count < 1)
    //            return false;

    //        money += cp.BidAsk.Bid;
    //        buyer.money -= cp.BidAsk.Bid;

    //        cp.BidAsk.Ask *= (float)(cp.BidAsk.Stocks[0].Percentage * 100.0f / (cp.BidAsk.Stocks.Count + 1));
    //        cp.BidAsk.Bid = cp.BidAsk.Ask * 1.001f;

    //        cp.BidAsk.Stocks[0].Owner = buyer;
    //        cp.BidAsk.Stocks.RemoveAt(0);

    //        cp.BidAsk.liquidity1m[cp.BidAsk.liquidity1m.Count - 1].BuyAmount++;
    //        return true;
    //    }

    //    public void SellStock(Stock stock)
    //    {
    //        if (stock.company.BidAsk == null)
    //            return;

    //        money += stock.company.BidAsk.Ask;
    //        stock.Owner.money += stock.company.BidAsk.Ask;

    //        stock.company.BidAsk.Ask /= (stock.company.BidAsk.Stocks[0].Percentage * 100.0f / (stock.company.BidAsk.Stocks.Count + 1));
    //        stock.company.BidAsk.Bid = stock.company.BidAsk.Ask * 1.001f;

    //        stock.Owner = null;
    //        stock.company.BidAsk.Stocks.Add(stock);
    //    }
    //}
    ////doel van exchange is om informatie te verkrijgen en bij te houden. Dit is de class waar andere classes "handelen", net als in een echte exchange
    //[Obsolete]
    //public class Exchange : IStockOwner, IExchange
    //{
    //    public List<Company> Companies = new List<Company>(); //lijst van alle geregistreerde bedrijven
    //    List<int> compconverter = new List<int>();
    //    public List<Trader> Traders = new List<Trader>(); //lijst van alle geregistreerde traders
    //    public List<OLD_BidAsk> BidAskSpreads { get; set; }//Lijst van alle stocks die te koop staan ingedeeld per bedrijf

    //    public List<Stock> Stocks
    //    {
    //        get
    //        {
    //            List<Stock> ret = new List<Stock>();
    //            foreach (var ba in BidAskSpreads)
    //            {
    //                ret.AddRange(ba.Stocks);
    //            }
    //            return ret;
    //        }
    //        set { }
    //    }
    //    public float money { get; set; }

    //    public void RegisterCompany(Company cp ,int part)
    //    {
    //        Companies.Add(cp);
    //        OLD_BidAsk bidAsk = new OLD_BidAsk(cp); //create new bidask
    //        BidAskSpreads.Add(bidAsk);
    //        cp.BidAsk = bidAsk;
    //    }
    //    public void RegisterTrader(Trader t)
    //    {
    //        Traders.Add(t);
    //    }

    //    public Exchange() { BidAskSpreads = new List<OLD_BidAsk>(); }


    //    Stock GetCheapestStock(Company cp)
    //    {
    //        if (!Companies.Contains(cp))
    //            return null;


    //        List<Stock> ls = BidAskSpreads[Companies.IndexOf(cp)].Stocks;
    //        Stock Cheapest = null;
    //        foreach (Stock stock in ls)
    //        {
    //            if (Cheapest == null || Cheapest.SellPrice > stock.SellPrice)
    //            {
    //                Cheapest = stock;
    //            }
    //        }
    //        return Cheapest;
    //    }

    //    //exchanging stocks
    //    public void SellStock(Stock stock)
    //    {
    //        List<Stock> ls = stock.company.BidAsk.Stocks;
    //        if (!ls.Contains(stock))
    //        {
    //            ls.Add(stock);
    //            stock.Owner = null;
    //        }
    //    }
    //    public bool BuyStock(Company cp, Trader buyer)
    //    {
    //        if (cp == null)
    //            return false;
    //        if (cp.BidAsk.Stocks.Count > 0)
    //        {
    //            //find cheapest stock
    //            Stock stock = cp.BidAsk.Stocks[0];
    //            cp.BidAsk.Stocks.RemoveAt(0);
    //            buyer.money -= cp.BidAsk.Bid;
    //            money += cp.BidAsk.Bid;

    //            //complete transaction
    //            buyer.AddStock(stock);

    //            cp.BidAsk.Bid *= 1.001f;

    //            return true;
    //        }
    //        return false;
    //    }
    //}
}
