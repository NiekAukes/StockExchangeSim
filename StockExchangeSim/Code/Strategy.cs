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
            List<Company> interestedCompanies = null;
            public PatternStrategy()
            {
                interestedCompanies = new List<Company>(Master.exchange.StocksForSale.Keys.ToList());
                while (interestedCompanies.Count > 10)
                {
                    interestedCompanies.RemoveAt(rn.Next(interestedCompanies.Count));
                }
            }
            public override void StrategyOutcome(Trader trader, Exchange exchange)
            {
                //price exploration
                double money = trader.money;
                double scale = 1;
                Company cp = interestedCompanies[rn.Next(interestedCompanies.Count)]; //invest in this company
               
                if (cp.stockPrices.Count < 51) //safeguard
                    return;

                List<StockPriceGraph> stockPrices = new List<StockPriceGraph>();
                for (int i = 0; i < 50; i++)
                {
                    stockPrices.Add(cp.stockPrices[cp.stockPrices.Count - 1 - (int)(i * scale)]);
                }

                //price discovery
                List<StockPriceGraph> highs = new List<StockPriceGraph>();
                List<StockPriceGraph> lows = new List<StockPriceGraph>();
                for (int i = 1; i < 49; i++)
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
                    return;
                for (float i = 0; i < avHighs.Count; i += 5.0f/avHighs.Count)
                {
                    avHighs.Add(highs[(int)i]);
                    avLows.Add(lows[(int)i]);
                }

                //channel pattern
                //            _              H  H-h  H-h  H-h
                //        _  / \/              L  L-h  L-h
                //    _  / \/            =========> breakout signal
                //   / \/
                //  /
                // /



                if (avHighs[4].High > avHighs[0].High && avLows[4].Low > avLows[0].Low)
                {
                    //first check
                    if (avHighs[2].High > avHighs[0].High && avLows[2].Low > avLows[0].Low)
                    {
                        //second check
                        if (avHighs[2].High < avHighs[4].High && avLows[2].Low < avLows[4].Low)
                        {
                            //Satisfied, BUY!!

                        }
                    }
                }


                //headnshoulders pattern
                //        _                 H  H-h  H-l
                //    _  / \  _               L   L-s
                //   / \/   \/ \          ======> Reversal signal
                //  /
                // /

                if (avLows[1].Low - avLows[3].Low < 0.01)
                {
                    //lows are same height
                    if (avHighs[1].High > avHighs[2].High && avHighs[2].High < avHighs[3].High)
                    {
                        //probably headnshoulders, try to sell at profit
                    }
                }

                //flag pattern
                //          Flat top        H  H-s  H-s
                //    _    _  _               L  L-h  L-h
                //   / \  / \/       ======> powerful break signal
                //  /   \/
                // /

                if (avHighs[0].High - avHighs[4].High < 0.02)
                {
                    //flat top, first satisfaction
                    if (avLows[0].Low < avLows[2].Low && avLows[2].Low < avLows[4].Low)
                    {
                        //FLAG!!! BUY BUY BUY!!!!!!!
                    }
                }

                //wedge pattern
                //          Flat top
                //    /\      
                //   /  \    /\  /\__       ======> direction of break uncertain
                //  /    \  /  \/
                // /      \/

                if (avHighs[0].High > avHighs[2].High && avHighs[2].High > avHighs[4].High)
                {
                    //decreasing highs
                    if (avLows[0].Low < avLows[2].Low && avLows[2].Low < avLows[4].Low)
                    {
                        //Wedge, maybe buy
                    }
                }

                //breakout fase
            }
        }
    }
}
