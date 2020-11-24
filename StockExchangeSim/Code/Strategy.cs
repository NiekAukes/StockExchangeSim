using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Input;

namespace Eco
{
    public partial class Trader
    {
        public abstract class Strategy
        {
            Random rn = new Random(Master.Seed);
            public abstract void StrategyOutcome(Trader trader, Exchange exchange);
        }

        public class SimpleStrategy : Strategy
        {
            public override void StrategyOutcome(Trader trader, Exchange exchange)
            {
                double money = trader.money;
                Company cp = exchange.Companies[rn.Next(exchange.StocksForSale.Count - 1)]; //invest in this company

                Stock s = exchange.GetCheapestStock(cp);
                if (s.SellPrice <= cp.Value * 0.01 * s.Percentage) //if stock is right price
                {
                    exchange.BuyStock(s, trader); //buy the stock
                    trader.ActionTime -= 3;
                }
                trader.ActionTime -= 5;

                
            }
        }
        public class PatternStrategy : Strategy
        {
            double scale = 1;
            List<Company> interestedCompanies = null;
            public PatternStrategy()
            {
                interestedCompanies = new List<Company>(Master.inst.exchange.Companies);
                while (interestedCompanies.Count > 10)
                {
                    interestedCompanies.RemoveAt(rn.Next(interestedCompanies.Count));
                }
            }

            
            bool Patterns(Company cp, Trader trader, Exchange exchange)
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
                for (int i = 0; i < 100; i++)
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
                for (int i = 1; i < 99; i++)
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
                            for (int i = 0; i < 10; i++)
                            {
                                Stock st = exchange.GetCheapestStock(cp);
                                if (exchange.BuyStock(st, trader)) //if transaction succeeded
                                    exchange.SellStock(st, st.SellPrice * 1.01);
                            }
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
                            Stock st = exchange.GetCheapestStock(cp);
                            if (st != null)
                            {
                                foreach (Stock stock in cpstocks)
                                    exchange.SellStock(stock, st.SellPrice);
                            }
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
                            Stock st = exchange.GetCheapestStock(cp);
                            if (exchange.BuyStock(st, trader)) //if transaction succeeded
                                exchange.SellStock(st, st.SellPrice * 1.02);
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
                                Stock st = exchange.GetCheapestStock(cp);
                                if (st != null)
                                {
                                    foreach (Stock stock in cpstocks)
                                        exchange.SellStock(stock, st.SellPrice * 1.005);
                                }
                            }
                        }
                        else
                        {
                            //buy
                            for (int i = 0; i < 10; i++)
                            {
                                Stock st = exchange.GetCheapestStock(cp);
                                if (exchange.BuyStock(st, trader)) //if transaction succeeded
                                    exchange.SellStock(st, st.SellPrice * 1.0015);
                            }
                        }
                    }
                }
                #endregion
                return Traded;
            }
            public override void StrategyOutcome(Trader trader, Exchange exchange)
            {
                //price exploration
                double money = trader.money;
                Company cp = interestedCompanies[rn.Next(interestedCompanies.Count)]; //invest in this company
               
                if (cp.stockPrices.Count < 101) //safeguard
                    return;


                bool Traded = false;
                if (rn.NextDouble() < trader.skill)
                {
                    Traded = Patterns(cp, trader, exchange);

                }
                if (!Traded)
                {
                    for (int i = 0; i < trader.Stocks.Count; i++) //check on stocks
                    {
                        exchange.SellStock(trader.Stocks[i], trader.Stocks[i].SellPrice / 2 +
                            (cp.Value * trader.Stocks[i].Percentage / trader.Stocks[i].SellPrice) / 2);
                    }

                    Stock st = exchange.GetCheapestStock(cp);

                    if ((cp.Competitiveness / 100) * (cp.Value * st.Percentage / st.SellPrice) > 1)
                    {

                        for (int i = 0; i < 10; i++)
                        {

                            if (exchange.BuyStock(st, trader)) //if transaction succeeded
                                exchange.SellStock(st, st.SellPrice * 1.01);
                        }
                    }
                    trader.ActionTime -= 150;
                }
                else
                    trader.ActionTime -= 300;


                //breakout fase
            }
        }
    }
}
