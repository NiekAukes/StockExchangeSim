using System;
using System.Collections.Generic;

namespace Eco
{

    public partial class Trader
    {

        public class StrategyFactory
        {
            Trader t = null;

            List<Strategy> multistratpool = null;
            List<Strategy> monostratpool = null;
            public StrategyFactory(Trader trader)
            {
                t = trader;
                multistratpool = new List<Strategy>()
                {
                    new BreakoutStrategy(t),
                    new TrendStrategy(t),
                    
                };
                monostratpool = new List<Strategy>()
                {
                    new MarketMakingStrategy(t),
                    new InvestorStrategy(t),
                };
            }
            public static int StrategyAmount = 2;
            public Strategy RandomStrategy(bool multistrat)
            {

                if (multistrat)
                {
                    int choice = rn.Next(multistratpool.Count);
                    Strategy ret = multistratpool[choice];
                    multistratpool.RemoveAt(choice);
                    ret.Init();
                    return ret;
                }
                else
                {
                    int choice = rn.Next(monostratpool.Count);
                    Strategy ret = monostratpool[choice];
                    ret.Init();
                    return ret;
                }

            }
        }

        public class MarketResults
        {//Float results 0 = will not buy, negative value = sell, positive value = buy
            public List<Tuple<Company, float>> Results = new List<Tuple<Company, float>>();
            public static MarketResults operator +(MarketResults a, MarketResults b)
            {
                List<Company> lscomp = new List<Company>();
                List<float> lsfloat = new List<float>();
                for (int i = 0; i < a.Results.Count; i++)
                {
                    lscomp.Add(a.Results[i].Item1);
                    lsfloat.Add(a.Results[i].Item2);
                }
                for (int i = 0; i < b.Results.Count; i++)
                {
                    if (!lscomp.Contains(b.Results[i].Item1))
                    {
                        lscomp.Add(b.Results[i].Item1);
                        lsfloat.Add(b.Results[i].Item2);
                    }
                    else
                    {
                        lsfloat[lscomp.IndexOf(b.Results[i].Item1)] += b.Results[i].Item2;
                    }
                }

                MarketResults MR = new MarketResults();
                for (int i = 0; i < lscomp.Count; i++)
                {
                    MR.Results.Add(new Tuple<Company, float>(lscomp[i], lsfloat[i]));
                }
                return MR;
            }
        }
        public abstract class Strategy
        {
            Random rn = new Random();
            public Trader trader = null;
            public abstract MarketResults StrategyOutcome(Trader trader, ECNBroker exchange);
            public abstract void Init();
        }
        [Obsolete]
        public class SimpleStrategy : Strategy
        {
            public override void Init()
            {
                throw new NotImplementedException();
            }

            public override MarketResults StrategyOutcome(Trader trader, ECNBroker exchange)
            {
                //float money = trader.money;
                //Company cp = exchange.Companies[rn.Next(exchange.BidAskSpreads.Count - 1)]; //invest in this company

                //Stock s = exchange.GetCheapestStock(cp);
                //if (s.SellPrice <= cp.Value * 0.01 * s.Percentage) //if stock is right price
                //{
                //    exchange.BuyStock(cp, trader); //buy the stock
                //    trader.ActionTime -= 3;
                //}
                //trader.ActionTime -= 5;
                return null;

            }
        }

        /*public class PatternStrategy : Strategy
        {
            float scale = 1;
            List<Company> interestedCompanies = null;
            public PatternStrategy()
            {
                interestedCompanies = Master.inst.GetAllCompanies();
                while (interestedCompanies.Count > 10)
                {
                    interestedCompanies.RemoveAt(rn.Next(interestedCompanies.Count));
                }
            }

            
            bool Patterns(Company cp, Trader trader, ExchangeBroker exchange)
            {


                #region Checks and Data Gathering
                bool OwnsStocks = false;
                List<Stock> cpstocks = null;
                for (int i = 0; i < trader.Stocks.Count; i++) //check on stocks
                {
                    if (trader.Stocks[i].company == cp)
                    {
                        OwnsStocks = true;
                        if (cpstocks == null)
                            cpstocks = new List<Stock>();
                        cpstocks.Add(trader.Stocks[i]);
                        break;
                    }
                }

                bool Traded = false;
                List<StockPriceGraph> stockPrices = new List<StockPriceGraph>();
                for (int i = 0; i < MinimumPriceCount; i++)
                {
                    StockPriceGraph spg = cp.stockPrices[cp.stockPrices.Count - 1 - (int)(i * scale)];
                    if (spg != null)
                        stockPrices.Add(spg);
                    else
                        i--;
                }

                #endregion

                #region price discovery
                List<StockPriceGraph> highs = new List<StockPriceGraph>();
                List<StockPriceGraph> lows = new List<StockPriceGraph>();
                for (int i = 1; i < MinimumPriceCount - 1; i++)
                {
                    if (stockPrices[i].Close > stockPrices[i - 1].Close &&
                            stockPrices[i].Close > stockPrices[i + 1].Close) //this is a high
                    {
                        highs.Add(stockPrices[i]);
                    }
                    if (highs.Count > 0) //first top always high
                    {
                        if (stockPrices[i].Close < stockPrices[i - 1].Close &&
                            stockPrices[i].Close < stockPrices[i + 1].Close) //this is a low
                        {
                            lows.Add(stockPrices[i]);
                        }
                    }
                }

                trader.ActionTime -= 45;

                List<StockPriceGraph> avHighs = new List<StockPriceGraph>();
                List<StockPriceGraph> avLows = new List<StockPriceGraph>();

                if (highs.Count < 5 || lows.Count < 5)
                    return Traded;
                for (float i = 0; i < highs.Count; i += 5.0f / highs.Count)
                {
                    avHighs.Add(highs[(int)i]);
                }
                for (float i = 0; i < lows.Count; i += 5.0f / lows.Count)
                {
                    avLows.Add(lows[(int)i]);
                }

                #endregion

                #region Pattern Recognition

                //channel pattern
                //            _              H  H-h  H-h  H-h
                //        _  / \/              L  L-h  L-h
                //    _  / \/            =========> breakout signal
                //   / \/
                //  /
                // /


                if (avHighs[0].High > avHighs[4].High && avLows[0].Low > avLows[4].Low)
                {
                    //first check
                    if (avHighs[2].High > avHighs[4].High && avLows[2].Low > avLows[4].Low)
                    {
                        //second check
                        if (avHighs[2].High < avHighs[0].High && avLows[2].Low < avLows[0].Low)
                        {
                            //Satisfied, BUY!!
                            Traded = true;
                            //for (int i = 0; i < 10; i++)
                            //{
                            //    Stock st = exchange.GetCheapestStock(cp);
                            //    if (exchange.BuyStock(st, trader)) //if transaction succeeded
                            //        exchange.SellStock(st, st.SellPrice * 1.01);
                            //}
                            MainPage.Log.Text += "Bought 10 stocks";
                        }
                    }
                }

                if (OwnsStocks)
                {
                    //headnshoulders pattern
                    //        _                 H  H-h  H-l
                    //    _  / \  _               L   L-s
                    //   / \/   \/ \          ======> Reversal signal
                    //  /
                    // /

                    if (Math.Abs(avLows[1].Low - avLows[3].Low) < 0.01)
                    {
                        //lows are same height
                        if (avHighs[3].High > avHighs[2].High && avHighs[2].High < avHighs[1].High)
                        {
                            //probably headnshoulders, try to sell at profit
                            Traded = true;
                            //Stock st = exchange.GetCheapestStock(cp);
                            //if (st != null)
                            //{
                            //    foreach (Stock stock in cpstocks)
                            //        exchange.SellStock(stock, st.SellPrice);
                            //}
                            MainPage.Log.Text += "sold stocks";

                        }
                    }
                }

                //flag pattern
                //          Flat top        H  H-s  H-s
                //    _    _  _               L  L-h  L-h
                //   / \  / \/       ======> powerful break signal
                //  /   \/
                // /
                
                if (Math.Abs(avHighs[0].High - avHighs[4].High) < 0.02)
                {
                    //flat top, first satisfaction
                    if (avLows[4].Low < avLows[2].Low && avLows[2].Low < avLows[0].Low)
                    {
                        //FLAG!!! BUY BUY BUY!!!!!!!
                        Traded = true;
                        for (int i = 0; i < 100; i++)
                        {
                            //Stock st = exchange.GetCheapestStock(cp);
                            exchange.BuyStock(cp, trader); //transaction succeeded
                        }
                    }
                }

                //wedge pattern
                //          
                //    /\      
                //   /  \    /\  __       ======> direction of break uncertain
                //  /    \  /  \/
                // /      \/

                if (avHighs[4].High > avHighs[2].High && avHighs[2].High > avHighs[0].High)
                {
                    //decreasing highs
                    if (avLows[4].Low < avLows[2].Low && avLows[2].Low < avLows[0].Low)
                    {
                        //Wedge, maybe buy
                        Traded = true;
                        if (rn.NextDouble() < trader.skill)
                        {
                            //sell
                            if (OwnsStocks)
                            {

                                foreach (Stock stock in cpstocks)
                                    exchange.SellStock(stock);
                            }
                        }
                        else
                        {
                            //buy
                            for (int i = 0; i < 10; i++)
                            {

                                exchange.BuyStock(cp, trader); //if transaction succeeded
                            }
                        }
                    }
                }
                #endregion
                return Traded;
            }
            public static int MinimumPriceCount = 50;
            public override MarketResults StrategyOutcome(Trader trader, ExchangeBroker exchange)
            {
                return null;
                //price exploration
                float money = trader.money;
                Company cp = interestedCompanies[rn.Next(interestedCompanies.Count)]; //invest in this company
               
                if (cp.stockPrices.Count < MinimumPriceCount + 1) //safeguard
                    return null;


                bool Traded = false;
                if (rn.NextDouble() < trader.skill)
                {
                    Traded = Patterns(cp, trader, exchange);

                }
                if (!Traded)
                {
                    //for (int i = 0; i < trader.Stocks.Count; i++) //check on stocks
                    //{
                    //    exchange.SellStock(trader.Stocks[i], trader.Stocks[i].SellPrice / 2 +
                    //        (cp.Value * trader.Stocks[i].Percentage / trader.Stocks[i].SellPrice) / 2);
                    //}

                    //Stock st = exchange.GetCheapestStock(cp);

                    //if (1 * (cp.Value * st.Percentage / st.SellPrice) > 1) //competitiveness
                    //{

                    //    for (int i = 0; i < 10; i++)
                    //    {

                    //        if (exchange.BuyStock(st, trader)) //if transaction succeeded
                    //            exchange.SellStock(st, st.SellPrice * 1.01);
                    //    }
                    //}
                    trader.ActionTime -= 150;
                }
                else
                    trader.ActionTime -= 300;


                //breakout fase
            }

            public override void Observe()
            {
                throw new NotImplementedException();
            }
        }*/
    }

}
